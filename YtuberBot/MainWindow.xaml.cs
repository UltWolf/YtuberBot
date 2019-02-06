using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Linq;
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
        Random r = new Random();
        public MainWindow()
        {
            InitializeComponent();
            User user = new User();
            if (File.Exists("user.data"))
            {
                try
                {
                    using (FileStream fs = new FileStream("user.data", FileMode.Open))
                    {

                        user = (User)bf.Deserialize(fs);
                    }
                    Login.Text = user.email;
                    Password.Text = user.password;

                }
                catch(Exception ex)
                {
                    MessageBox.Show("File can`t found, or stream is empty");
                }
                }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FirefoxDriver driver = new FirefoxDriver();
            LoginInAccount(driver);

            driver.Navigate().GoToUrl("https://ytuber.ru/work/view");
            Thread.Sleep(r.Next(1, 10) * 1000);
            if (driver.Url != "https://ytuber.ru/work/view") {
                LoginInAccount(driver);
            } ;

            By className = By.ClassName("table-responsive");
            var tables = driver.FindElement(className);
            By tbody = By.TagName("tbody");
            var body = tables.FindElement(tbody); 
            foreach ( var b in body.FindElements(By.TagName("tr")))
            {
                if (b.GetAttribute("class") != "success")
                {
                    By ahref = By.TagName("a");
                    var href = b.FindElement(ahref);
                    By imgTag = By.TagName("img");
                    var img = href.FindElement(imgTag);
                    img.Click();

                    By time = By.ClassName("time");

                    var windows = driver.WindowHandles.ToList();
                    if (windows.Count == 1)
                    {
                        continue;
                    }
                    Thread.Sleep(int.Parse(b.FindElement(time).Text) * 1000);
                    try
                    {
                        driver.SwitchTo().Window(windows[1]);
                        driver.Close();
                    }
                    catch (Exception ex)
                    {

                    }
                    driver.SwitchTo().Window(windows[0]);
                    By agree = By.ClassName("btn btn-info btn-circle");

                    b.FindElement(agree).Click();
                    Thread.Sleep(r.Next(1, 10) * 1000);
                }
            }
        }

        public void LoginInAccount(FirefoxDriver driver)
        {
            driver.Url = "https://ytuber.ru";

            User user = new User();
            user.email = Login.Text;
            user.password = Password.Text;
            using (FileStream fs = new FileStream("user.data", FileMode.Create))
            {

                bf.Serialize(fs, user);
            } 
            By formGroup = By.ClassName("form-group");
            By inputTag = By.TagName("input");

            driver.Navigate().GoToUrl("https://ytuber.ru/auth/login");
            if (IsGoogle.IsChecked == true)
            {

                Thread.Sleep(r.Next(4, 10) * 1000);
                driver.FindElements(formGroup)[3].FindElement(By.TagName("button")).Submit();
                Thread.Sleep(r.Next(5, 10) * 1000);
                driver.FindElement(By.Id("identifierId")).SendKeys(Login.Text);
                driver.FindElement(By.Id("identifierNext")).Click();

                Thread.Sleep(r.Next(4, 10) * 1000);
                var inputs = driver.FindElements(inputTag);

                foreach( var i in inputs)
                {
                    if(i.GetAttribute("name")== "password")
                    {
                        driver.FindElements(inputTag)[1].SendKeys(Password.Text);
                        driver.FindElement(By.Id("passwordNext")).Click();
                        Thread.Sleep(r.Next(4, 10) * 1000);
                        break;
                    }
                }
               
            }
            else
            {

                driver.FindElements(formGroup)[0].FindElement(inputTag).SendKeys(Login.Text);
                driver.FindElements(formGroup)[1].FindElement(inputTag).SendKeys(Login.Text);
                driver.FindElements(formGroup)[2].FindElement(inputTag).Submit();
            }
        }
    }
}
