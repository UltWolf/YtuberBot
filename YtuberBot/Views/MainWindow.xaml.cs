using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using YtuberBot.Models;
using YtuberBot.Views;

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
            FunctionalBlock.Visibility = Visibility.Collapsed;
            LoadingBlock.Visibility = Visibility.Visible;
            FirefoxDriver driver = new FirefoxDriver(FirefoxDriverService.CreateDefaultService(),new FirefoxOptions(),TimeSpan.FromMinutes(5));
            Thread thread = new Thread(() => LoginInAccount(driver));
            Storyboard sb = (Storyboard)this.LoadingIcon.FindResource("spin");
            sb.Begin();
            sb.SetSpeedRatio(27);
            thread.Start();
            ManagmentView mv = new ManagmentView(driver);
        }

        public void LoginInAccount(FirefoxDriver driver)
        { 

            User user = new User();
            bool GoogleAuthentication = true;
            Dispatcher.Invoke(() => {
                user.email = Login.Text;
                user.password = Password.Text;
                GoogleAuthentication = (bool)IsGoogle.IsChecked;
            });

            using (FileStream fs = new FileStream("user.data", FileMode.Create))
            {

                bf.Serialize(fs, user);
            } 
            By formGroup = By.ClassName("form-group");
            By inputTag = By.TagName("input");
            

            driver.Navigate().GoToUrl("https://ytuber.ru/auth/login");  
            if (GoogleAuthentication)
            {

                Thread.Sleep(r.Next(4, 10) * 1000);
                Dispatcher.Invoke(() =>
                {
                    LoadingText.Content = "Login to Google Account";
                });
                driver.FindElements(formGroup)[3].FindElement(By.TagName("button")).Submit();
                Thread.Sleep(r.Next(30, 40) * 1000);
                
                Dispatcher.Invoke(() =>
                {
                    LoadingText.Content = "Input login element";
                    driver.FindElement(By.Id("identifierId")).SendKeys(Login.Text);
                });
                driver.FindElement(By.Id("identifierNext")).Click();

                Thread.Sleep(r.Next(30, 40) * 1000);
                var inputs = driver.FindElements(inputTag);

                foreach( var i in inputs)
                {
                    if(i.GetAttribute("name")== "password")
                    {
                        Dispatcher.Invoke(() =>
                        {
                            LoadingText.Content = "Input password element";
                            driver.FindElements(inputTag)[1].SendKeys(Password.Text);
                        });
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
            Dispatcher.Invoke(() =>
            {
                LoadingText.Content = "Final element";
            });
                Dispatcher.Invoke(() => {
                ManagmentView mw = new ManagmentView(driver);
                mw.Show();
                this.Close();
            });
         
           
        }
    }
}
