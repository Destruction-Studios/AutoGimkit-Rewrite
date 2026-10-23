namespace AutoGimkit
{
    public partial class Form1 : Form
    {
        public HomeScreen homeScreen { get; private set; }
        public MainScreen mainScreen { get; private set; }

        public Form1()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            TopMost = true;
            homeScreen = new HomeScreen(this);
            mainScreen = new MainScreen(this);

            ShowPage(homeScreen);
        }

        private void ShowPage(UserControl page)
        {
            panelContainer.Controls.Clear();
            page.Dock = DockStyle.Fill;
            panelContainer.Controls.Add(page);
        }

        public void ShowHome()
        {
            ShowPage(homeScreen);
        }

        public void ShowMain()
        {
            ShowPage(mainScreen);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Opening Browser");
            //Task.Run(() => Program.CreateBrowserAsync());
            await Program.CreateBrowserAsync();

            Console.WriteLine("End");
        }
    }
}
