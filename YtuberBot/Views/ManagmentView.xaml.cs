using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace YtuberBot.Views
{
    /// <summary>
    /// Логика взаимодействия для ManagmentView.xaml
    /// </summary>
    public partial class ManagmentView : Window
    {
        private readonly FirefoxDriver _fd;
        private readonly Random r = new Random();
        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        CancellationToken token;
        private string LikeValue = "";
        Task task;

        public ManagmentView(FirefoxDriver fd)
        {
            InitializeComponent();
            _fd = fd;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FunctionalBlock.Visibility = Visibility.Collapsed;
            StopBlock.Visibility = Visibility.Visible;
            Task task = new Task(()=>Navigate());
            task.Start();
            
           
           
        }
        private void Navigate()
        {
            _fd.Navigate().GoToUrl("https://ytuber.ru/work/view");
            Task task = new Task(() => DoTask(Watch, "View"));

            token = cancelTokenSource.Token;

            task.Start();
        }
        public void DoTask(Action<IWebElement> action,string taskUrl)
        {  
            Thread.Sleep(r.Next(1, 10) * 1000); 
            By className = By.ClassName("table-responsive");
            var tables = _fd.FindElement(className);
            By tbody = By.TagName("tbody");
            var body = tables.FindElement(tbody);
            foreach (var b in body.FindElements(By.TagName("tr")))
            {
                
                if (b.GetAttribute("class") != "success")
                {
                    action.Invoke(b);
                    if (token.IsCancellationRequested)
                    {
                        FunctionalBlock.Visibility = Visibility.Visible;
                        StopBlock.Visibility = Visibility.Collapsed;
                        return;
                    }
                    Thread.Sleep(r.Next(1, 10) * 1000);
                }
            }

            var paginationClass = _fd.FindElementByClassName("pagination");
            var buttons = paginationClass.FindElements(By.ClassName("paginate_button"));
            var url = this._fd.Url.Split('/');
            try
            {
                var countPages = int.Parse(url[url.Length - 1]) + 12;
                _fd.Navigate().GoToUrl("https://ytuber.ru/work/" + taskUrl + "/" + countPages);

            }
            catch (FormatException fx)
            {
                _fd.Navigate().GoToUrl("https://ytuber.ru/work/"+taskUrl+"/12");
            }
            DoTask(action,taskUrl);
        }
         public void Like(IWebElement b)
        {
            By ahref = By.TagName("a");
            var href = b.FindElement(ahref);
            By imgTag = By.TagName("img");
            var img = href.FindElement(imgTag);
            var typeLike = b.FindElements(By.TagName("td"))[4].FindElement(By.TagName("span")).Text;
            img.Click(); 
            By time = By.ClassName("time"); 
            var windows = _fd.WindowHandles.ToList(); 
            Thread.Sleep(int.Parse(b.FindElement(time).Text) * 1000);
            try
            {

                _fd.SwitchTo().Window(windows[1]); 
                
            var buttonPanel =  _fd.FindElementById("top-level-buttons");
            var buttons =  buttonPanel.FindElements(By.TagName("ytd-toggle-button-renderer"));
           
            if ( typeLike=="Лайк")
            {
                buttons[0].Click();
            }
            else
            {
                buttons[1].Click();
            }
                _fd.Close();
            }
            catch (Exception ex)
            {

            }
            _fd.SwitchTo().Window(windows[0]);
            By agree = By.ClassName("btn");

            try
            {
                var elementClick = b.FindElement(agree);
                elementClick.Click();
            }
            catch (Exception ex)
            {

            }
        }

        public void Watch(IWebElement b)
        {
            By ahref = By.TagName("a");
            var href = b.FindElement(ahref);
            By imgTag = By.TagName("img");
            var img = href.FindElement(imgTag);
            img.Click();

            By time = By.ClassName("time");

            var windows = _fd.WindowHandles.ToList();
            
            Thread.Sleep(int.Parse(b.FindElement(time).Text) * 1000);
            try
            {

                _fd.SwitchTo().Window(windows[1]); 
                _fd.Close();
            }
            catch (Exception ex)
            {

            }
            _fd.SwitchTo().Window(windows[0]);
            By agree = By.ClassName("btn");

            try
            {
                var elementClick = b.FindElement(agree);
                elementClick.Click();
            }
            catch (Exception ex)
            {

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _fd.Navigate().GoToUrl("https://ytuber.ru/work/like");
            task = new Task(() => DoTask(Like,"like"));
            FunctionalBlock.Visibility = Visibility.Collapsed;
            StopBlock.Visibility = Visibility.Visible;
            task.Start();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _fd.Navigate().GoToUrl("https://ytuber.ru/work/comment");
            task = new Task(() => DoTask(Comment, "comment"));
            FunctionalBlock.Visibility = Visibility.Collapsed;
            StopBlock.Visibility = Visibility.Visible;
            task.Start();

        }
        private void Comment(IWebElement b)
        {
            try
            {
                By ahref = By.TagName("a");
                var href = b.FindElement(ahref);
                By imgTag = By.TagName("img");
                var img = href.FindElement(imgTag);
                var typeComment = b.FindElements(By.TagName("td"))[4].FindElement(By.TagName("span")).Text;
                bool pasteComment = false;
                string comment = "";
                if (typeComment == "Позитивный")
                {
                    Dispatcher.Invoke(() =>
                    {
                        comment = Positive.Text;
                    });
                }
                else if (typeComment == "Негативный")
                {
                    Dispatcher.Invoke(() =>
                    {
                        comment = Negative.Text;
                    });
                }
                else if ((typeComment == "Произвольный") || (typeComment == "-"))
                {
                    Dispatcher.Invoke(() =>
                    {
                        comment = Any.Text;
                    });
                }
                else
                {
                    b.FindElements(By.TagName("td"))[4].FindElement(By.TagName("a")).Click();
                    pasteComment = true;
                }
                img.Click();

                By time = By.ClassName("time");

                var windows = _fd.WindowHandles.ToList();

                try
                {
                    _fd.SwitchTo().Window(windows[1]);
                    _fd.FindElement(By.TagName("paper-input-container")).Click();
                    if (pasteComment)
                    {
                        _fd.FindElement(By.Id("contenteditable-textarea")).SendKeys(Clipboard.GetText());
                    }
                    else
                    {
                        _fd.FindElement(By.Id("contenteditable-textarea")).SendKeys(comment);
                    }
                    _fd.FindElementById("creation-box").FindElement(By.TagName("paper-button")).Click();
                    Thread.Sleep(int.Parse(b.FindElement(time).Text) * 1000);
                    _fd.Close();
                }
                catch (Exception ex)
                {

                }
                _fd.SwitchTo().Window(windows[0]);
                By agree = By.ClassName("btn");

                try
                {
                    var elementClick = b.FindElement(agree);
                    elementClick.Click();
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex) { };

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            _fd.Navigate().GoToUrl("https://ytuber.ru/work/comment_like");
            task = new Task(() => DoTask(LikeToComment, "comment_like"));
            FunctionalBlock.Visibility = Visibility.Collapsed;
            StopBlock.Visibility = Visibility.Visible;
            task.Start();
        }
        private void LikeToComment(IWebElement b)
        {

            By ahref = By.TagName("a");
            var href = b.FindElement(ahref);
            By imgTag = By.TagName("img");
            var img = href.FindElement(imgTag);
            img.Click();

            By time = By.ClassName("time");

            var windows = _fd.WindowHandles.ToList();

            Thread.Sleep(int.Parse(b.FindElement(time).Text) * 1000);
            try
            {

                _fd.SwitchTo().Window(windows[1]);
                _fd.FindElementByTagName("ytd-comment-renderer").FindElement(By.Id("like-button")).Click();
                _fd.Close();
            }
            catch (Exception ex)
            {

            }
            _fd.SwitchTo().Window(windows[0]);
            By agree = By.ClassName("btn");

            try
            {
                var elementClick = b.FindElement(agree);
                elementClick.Click();
            }
            catch (Exception ex)
            {

            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            cancelTokenSource.Cancel();
        }
    }
}
