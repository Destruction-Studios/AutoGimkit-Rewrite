//Converted from python to C# with claude.ai


using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace AutoGimkit
{
    /// <summary>One Latin -> English entry.</summary>
    public sealed record WordEntry(string Latin, string English);

    public enum AddResult { Added, Updated, Unchanged }

    /// <summary>
    /// A Latin -> English word database that loads and saves itself to words.json.
    ///
    ///   using var words = new WordDb();           // loads or creates words.json
    ///   words.Add("aqua", "water");               // saved automatically -> Added
    ///   words.Add("aqua", "the water");           // same word, new meaning -> Updated
    ///   words.Get("aqua");                        // WordEntry { Latin = aqua, English = water }
    ///   words.Get("aquaa");                       // null (exact matches only)
    ///
    /// Lookups are exact, apart from ignoring surrounding whitespace and letter case.
    ///
    /// Saves are atomic (temp file + replace), batched (many rapid changes become
    /// one write after `delay` seconds), and anything unsaved is flushed on exit.
    /// </summary>
    public sealed class WordDb : IDisposable, IEnumerable<string>
    {
        /// <summary>words.json next to the executable.</summary>
        public static readonly string DefaultPath =
            Path.Combine(AppContext.BaseDirectory, "words.json");

        // Produces the same format as the Python version:
        // { "aqua": { "latin": "aqua", "english": "water" } }
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // don't escape non-ASCII letters
        };

        private readonly string _path;
        private readonly TimeSpan _delay;
        private readonly object _lock = new(); // C# locks are re-entrant, like Python's RLock
        private System.Threading.Timer? _timer;
        private bool _dirty;
        private Dictionary<string, WordEntry> _words;

        /// <param name="path">JSON file to read from / write to.</param>
        /// <param name="delaySeconds">
        /// Seconds to wait before writing after a change. 0 = write immediately on every change.
        /// </param>
        public WordDb(string? path = null, double delaySeconds = 0.5)
        {
            _path = path ?? DefaultPath;
            _delay = TimeSpan.FromSeconds(delaySeconds);
            _words = Load();

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }

        // -- loading / saving -------------------------------------------------

        private Dictionary<string, WordEntry> Load()
        {
            string problem;
            try
            {
                string json = File.ReadAllText(_path, Encoding.UTF8);
                var raw = JsonSerializer.Deserialize<Dictionary<string, WordEntry>>(json, JsonOptions);
                if (raw != null)
                    return raw;
                problem = "not a JSON object";
            }
            catch (FileNotFoundException) { return new(); }
            catch (DirectoryNotFoundException) { return new(); }
            catch (JsonException) { problem = "invalid JSON"; } // also covers an empty file

            // Keep the bad file instead of silently overwriting it.
            string backup = _path + ".corrupt";
            File.Move(_path, backup, overwrite: true);
            Debug.WriteLine($"{_path} was {problem}, moved to {backup} and reset");
            return new();
        }

        public void LoadFromPastebin(string rawJson)
        {
            Debug.WriteLine("Parsing raw json");
            Dictionary<string, WordEntry>? newWords = null;
            try
            {
                var raw = JsonSerializer.Deserialize<Dictionary<string, WordEntry>>(rawJson, JsonOptions);
                if (raw != null)
                    newWords = raw;
            } catch (JsonException ex)
            {
                Debug.WriteLine(ex.Message);
            }

            if (newWords != null)
            {
                _words = newWords;
                Debug.WriteLine("Successfully wrote new lines");
                Changed();
            }
        }

        private void Changed()
        {
            lock (_lock)
            {
                _dirty = true;
                if (_delay <= TimeSpan.Zero)
                    Save();
                else
                    _timer ??= new System.Threading.Timer(
                        _ => TimerSave(), null, _delay, Timeout.InfiniteTimeSpan);
            }
        }

        private void TimerSave()
        {
            try
            {
                Flush(); // not Save(): a manual Save may have already written everything
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Autosave to {_path} failed: {ex}");
            }
        }

        /// <summary>Write to disk right now (atomic).</summary>
        public void Save()
        {
            lock (_lock)
            {
                _timer?.Dispose();
                _timer = null;

                string text = JsonSerializer.Serialize(_words, JsonOptions);

                string fullPath = Path.GetFullPath(_path);
                string dir = Path.GetDirectoryName(fullPath)!;
                Directory.CreateDirectory(dir);

                string tmp = Path.Combine(dir, $"{Path.GetFileName(fullPath)}.{Guid.NewGuid():N}.tmp");
                try
                {
                    using (var fs = new FileStream(tmp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        byte[] bytes = new UTF8Encoding(false).GetBytes(text);
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Flush(flushToDisk: true); // like os.fsync
                    }
                    File.Move(tmp, fullPath, overwrite: true); // like os.replace
                }
                catch
                {
                    try { File.Delete(tmp); } catch { /* ignore */ }
                    throw;
                }

                _dirty = false;
            }
        }

        

        /// <summary>Save only if there are unsaved changes.</summary>
        public void Flush()
        {
            lock (_lock)
            {
                if (_dirty)
                    Save();
            }
        }

        private void OnProcessExit(object? sender, EventArgs e)
        {
            try { Flush(); }
            catch (Exception ex) { Debug.WriteLine($"Flush on exit failed: {ex}"); }
        }

        public void Dispose()
        {
            AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
            Flush();
            lock (_lock)
            {
                _timer?.Dispose();
                _timer = null;
            }
        }

        // -- words ------------------------------------------------------------

        /// <summary>Normalise a word for lookup: trimmed and lowercase.</summary>
        private static string Key(string latin) => latin.Trim().ToLowerInvariant();

        /// <summary>
        /// Save a word. If the exact word is already stored, its entry is overwritten
        /// with the new meaning.
        /// Returns Added, Updated (existed with a different meaning),
        /// or Unchanged (already stored exactly like this).
        /// </summary>
        public AddResult Add(string latin, string english)
        {
            string key = Key(latin);
            var newEntry = new WordEntry(key, english.Trim().ToLowerInvariant());

            lock (_lock)
            {
                AddResult status;
                if (!_words.TryGetValue(key, out var oldEntry))
                    status = AddResult.Added;
                else if (oldEntry == newEntry) // records compare by value
                    return AddResult.Unchanged;
                else
                    status = AddResult.Updated;

                _words[key] = newEntry;
                Changed();
                return status;
            }
        }

        /// <summary>Returns the entry, or null if the word isn't stored. (Records are immutable, so no copy is needed.)</summary>
        public WordEntry? Get(string latin)
        {
            lock (_lock)
                return _words.TryGetValue(Key(latin), out var entry) ? entry : null;
        }

        public bool Remove(string latin)
        {
            lock (_lock)
            {
                if (!_words.Remove(Key(latin)))
                    return false;
                Changed();
                return true;
            }
        }

        public bool Contains(string latin)
        {
            lock (_lock)
                return _words.ContainsKey(Key(latin));
        }

        public int Count
        {
            get { lock (_lock) return _words.Count; }
        }

        /// <summary>Iterates over a snapshot of the stored words, so changing the db mid-loop is safe.</summary>
        public IEnumerator<string> GetEnumerator()
        {
            List<string> snapshot;
            lock (_lock)
                snapshot = _words.Keys.ToList();
            return snapshot.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}