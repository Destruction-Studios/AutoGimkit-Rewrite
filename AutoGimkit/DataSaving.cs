using System.Text.Json;
using System.Text.Json.Serialization;

namespace AutoGimkit
{
    public class SaveData
    {
        public string GimkitId { get; set; } = string.Empty;
        public int incorrectMin { get; set; } = 0;
        public int incorrectMax { get; set; } = 1;
        public float speedMultiplier { get; set; } = 1f;
    }

    internal class DataSaving
    {
        private string filePath;
        private SaveData saveData;
        private bool isDirty;

        public DataSaving(string filePath)
        {
            this.filePath = filePath;
            saveData = new SaveData();
        }

        #region Getters and Setters

        public void SetGimkitId(string id)
        {
            isDirty = true;
            this.saveData.GimkitId = id;
        }
        public string GetGimkitId() => this.saveData.GimkitId;

        public void SetIncorrectMin(int min)
        {
            isDirty = true;
            this.saveData.incorrectMin = min;
        }
        public int GetIncorrectMin() => this.saveData.incorrectMin;

        public void SetIncorrectMax(int max)
        {
            isDirty = true;
            this.saveData.incorrectMax = max;
        }
        public int GetIncorrectMax() => this.saveData.incorrectMax;

        public void SetSpeedMultiplier(float speedMulti)
        {
            isDirty = true;
            this.saveData.speedMultiplier = MathF.Max(0, speedMulti);
        }
        public float GetSpeedMultiplier() => MathF.Max(0, this.saveData.speedMultiplier);
        #endregion

        public bool IsDirty() => this.isDirty;

        public void WriteData()
        {
            isDirty = false;
            string json = JsonSerializer.Serialize(saveData);
            File.WriteAllText(filePath, json);
            //Console.WriteLine(File.ReadAllText(filePath));
        }

        public void LoadData()
        {
            if (!File.Exists(filePath))
                return;

            try
            {
                string json = File.ReadAllText(filePath);
                saveData = JsonSerializer.Deserialize<SaveData>(json) ?? new SaveData();
            }
            catch (JsonException)
            {
                saveData = new SaveData();
            }

            isDirty = false;
        }
    }
}
