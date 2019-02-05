using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using YtuberBot.Models;

namespace YtuberBot
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BinaryFormatter bf = new BinaryFormatter();
        public MainWindow()
        {
            InitializeComponent();
            User user = new User();
            if (File.Exists("user.data"))
            {
                using (FileStream fs = new FileStream("user.data", FileMode.Open))
                {

                    user = (User)bf.Deserialize(fs);
                }
                Login.Text = user.email;
                Password.Text = user.password;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IWebDriver driver = new FirefoxDriver();
            By className = By.ClassName("table-responsive");
            driver.Url = "https://ytuber.ru";
        
            User user = new User();
            user.email = Login.Text;
            user.password = Password.Text;
            using(FileStream fs = new FileStream("user.data", FileMode.Create))
            {

                bf.Serialize(fs,user);
            }

            By formGroup = By.ClassName("form-group");  
            By inputTag = By.TagName("input");
            driver.Navigate().GoToUrl("https://ytuber.ru/work/view");
            if (IsGoogle.IsChecked == true)
            {
                Thread.Sleep(10000);
                driver.FindElements(formGroup)[3].FindElement(By.TagName("button")).Submit();
                Thread.Sleep(10000);
                driver.FindElement(By.Id("identifierId")).SendKeys(Login.Text);
                driver.FindElement(By.Id("identifierNext")).Click();
                Thread.Sleep(10000);
                driver.FindElement(inputTag).SendKeys(Password.Text);

            }
            else
            {

                driver.FindElements(formGroup)[0].FindElement(inputTag).SendKeys(Login.Text);
                driver.FindElements(formGroup)[1].FindElement(inputTag).SendKeys(Login.Text);
                driver.FindElements(formGroup)[2].FindElement(inputTag).Submit();
            }
            var tables = driver.FindElement(className);
            By tbody = By.TagName("tbody");
            var body = tables.FindElements(tbody);
            foreach( var b in body)
            {
                By ahref = By.TagName("a");
                var href = b.FindElement(ahref);
                By imgTag = By.TagName("img");
                var img = href.FindElement(imgTag);
                img.Click();
                
            }

        }
    }
}
