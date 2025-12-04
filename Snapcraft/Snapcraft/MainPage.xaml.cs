using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Snapcraft
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {


        public MainPage()
        {
            this.InitializeComponent();
            // the PromptTextBOX will automatically receive focus after the page loaded
            this.Loaded += (sender, e) => PromptTextBox.Focus(FocusState.Programmatic);
            //使用 Frame 的 NavigationCacheMode 属性，设置为 Required，这样在返回时页面不会重新加载。
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        //API
        private async void OnGenerateImageClick(object sender, RoutedEventArgs e)
        {
            string prompt = PromptTextBox.Text;
            string apiUrl = "https://api.stability.ai/v2beta/stable-image/generate/sd3";
            string apiToken = "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";  // 替换为你的API Token



            // Display loading
            LoadingRing.IsActive = true;
            GenerateImage.IsEnabled = false;

            // Clear previous images
            ImageContainer.Children.Clear();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // 设置Authorization头
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "sk-sgLB41jpU7UlgHig75o0GJ7m16rYgWWwAAEjeXIR6oN6093S");

                    // 设置Accept头，表明接受的内容类型为图片
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("image/*"));
                    
                    for (int i = 0; i < 3; i++)
                    {
                        int randomSeed = new Random().Next();

                        using (var formData = new MultipartFormDataContent())
                        {
                            formData.Add(new StringContent(prompt), "\"prompt\"");
                            formData.Add(new StringContent(randomSeed.ToString()), "\"seed\"");
                            formData.Add(new StringContent("512"), "\"width\"");
                            formData.Add(new StringContent("512"), "\"height\"");
                            formData.Add(new StringContent("1"), "\"samples\"");
                            formData.Add(new StringContent("7.5"), "\"cfg_scale\"");
                            formData.Add(new StringContent("50"), "\"steps\"");

                            HttpResponseMessage response = await client.PostAsync(apiUrl, formData);

                            if (response.IsSuccessStatusCode)
                            {
                                var imageStream = await response.Content.ReadAsStreamAsync();
                                BitmapImage bitmapImage = new BitmapImage();
                                await bitmapImage.SetSourceAsync(imageStream.AsRandomAccessStream());

                                // 创建Image控件
                                Image image = new Image
                                {
                                    Source = bitmapImage,
                                    Stretch = Stretch.UniformToFill
                                };

                                // every time image has been tapped, will execute the method
                                image.Tapped += Image_Click;

                                // 创建带圆角的Border控件
                                Border border = new Border
                                {
                                    CornerRadius = new CornerRadius(20),
                                    Child = image,
                                    Margin = new Thickness(10)
                                };

                                int row = i / 2;
                                int column = i % 2;
                                Grid.SetRow(border, row);
                                Grid.SetColumn(border, column);

                                ImageContainer.Children.Add(border);

                                // Hide the ImageTextGroup
                                ImageTextGroup.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                string errorMessage = await response.Content.ReadAsStringAsync();
                                await ShowErrorDialog("Error: " + errorMessage);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowErrorDialog("An error occurred: " + ex.Message);
            }
            finally
            {
                LoadingRing.IsActive = false;
                GenerateImage.IsEnabled = true;
            }
        }

         //API2
        //private async void OnGenerateImageClick(object sender, RoutedEventArgs e)
        //{
        //    string prompt = PromptTextBox.Text;
        //    string apiUrl = "https://api-inference.huggingface.co/models/nerijs/animation2k-flux";
        //    string apiToken = "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";  // 替换为你的API Token



        //    // Display loading
        //    LoadingRing.IsActive = true;
        //    GenerateImage.IsEnabled = false;

        //    // Clear previous images
        //    ImageContainer.Children.Clear();

        //    try
        //    {
        //        using (HttpClient client = new HttpClient())
        //        {
        //            // 设置Authorization头
        //            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);

        //            // 设置Accept头，表明接受的内容类型为图片
        //            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("image/*"));

        //            for (int i = 0; i < 3; i++)
        //            {
        //                int randomSeed = new Random().Next();

        //                using (var formData = new MultipartFormDataContent())
        //                {
        //                    formData.Add(new StringContent(prompt), "\"prompt\"");
        //                    formData.Add(new StringContent(randomSeed.ToString()), "\"seed\"");
        //                    formData.Add(new StringContent("512"), "\"width\"");
        //                    formData.Add(new StringContent("512"), "\"height\"");
        //                    formData.Add(new StringContent("1"), "\"samples\"");
        //                    formData.Add(new StringContent("7.5"), "\"cfg_scale\"");
        //                    formData.Add(new StringContent("50"), "\"steps\"");

        //                    HttpResponseMessage response = await client.PostAsync(apiUrl, formData);

        //                    if (response.IsSuccessStatusCode)
        //                    {
        //                        var imageStream = await response.Content.ReadAsStreamAsync();
        //                        BitmapImage bitmapImage = new BitmapImage();
        //                        await bitmapImage.SetSourceAsync(imageStream.AsRandomAccessStream());

        //                        // 创建Image控件
        //                        Image image = new Image
        //                        {
        //                            Source = bitmapImage,
        //                            Stretch = Stretch.UniformToFill
        //                        };

        //                        // every time image has been tapped, will execute the method
        //                        image.Tapped += Image_Click;

        //                        // 创建带圆角的Border控件
        //                        Border border = new Border
        //                        {
        //                            CornerRadius = new CornerRadius(20),
        //                            Child = image,
        //                            Margin = new Thickness(10)
        //                        };

        //                        int row = i / 2;
        //                        int column = i % 2;
        //                        Grid.SetRow(border, row);
        //                        Grid.SetColumn(border, column);

        //                        ImageContainer.Children.Add(border);

        //                        // Hide the ImageTextGroup
        //                        ImageTextGroup.Visibility = Visibility.Collapsed;
        //                    }
        //                    else
        //                    {
        //                        string errorMessage = await response.Content.ReadAsStringAsync();
        //                        await ShowErrorDialog("Error: " + errorMessage);
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await ShowErrorDialog("An error occurred: " + ex.Message);
        //    }
        //    finally
        //    {
        //        LoadingRing.IsActive = false;
        //        GenerateImage.IsEnabled = true;
        //    }
        //}


        private async System.Threading.Tasks.Task ShowErrorDialog(string message)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(message);
            await dialog.ShowAsync();
        }

        //Menu tab
        private void OnTabClick(object sender, RoutedEventArgs e)
        {
            ResetAllEllipses();

            var button = sender as Button;
            int index = int.Parse(button.Tag.ToString());

            Ellipse ellipse = (Ellipse)FindName($"Ellipse{index}");
            // 通过 ARGB 直接创建颜色
            Color customColor = Color.FromArgb(255, 232, 72, 40); // the first 255 indecate Appha = 100
            ellipse.Stroke = new SolidColorBrush(customColor);
            ellipse.StrokeThickness = 1;
        }

        private void ResetAllEllipses()
        {
            for (int i = 0; i < 3; i++)
            {
                Ellipse ellipse = (Ellipse)FindName($"Ellipse{i}");
                ellipse.Stroke = new SolidColorBrush(Colors.Transparent);
                ellipse.StrokeThickness = 0;
            }
        }

        //Open Upgrade Window
        private void SnapcraftUpgrade_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Upgrade));

        }

        // Click pic then go to the PicDetail page
        private void Image_Click(object sender, RoutedEventArgs e)
        {
            //将SENDER转化为image控件
            var image = sender as Image;
            //将image控件转换为bitmapImage
            var bitmapImage = image.Source as BitmapImage;

            Frame.Navigate(typeof (PicDetail), bitmapImage);
        }



        // Button size change for style
        private void Button_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var button = sender as Button;
            button.Height = button.ActualWidth;

        }

        // show the style panel
        private void ShowStylesButton_Click(object sender, RoutedEventArgs e)
        {
            StyleGroup.Visibility = Visibility.Visible;
        }

        // hide the style panel
        private void InternalButton_Click(object sender, RoutedEventArgs e)
        {
            StyleGroup.Visibility = Visibility.Collapsed;
        }

        // show the size panel
        private void ShowSizeButton_Click(object sender, RoutedEventArgs e)
        {
            SizeGroup.Visibility = Visibility.Visible;
        }

        // hide the style panel
        private void InternalSizeButton_Click(object sender, RoutedEventArgs e)
        {
            SizeGroup.Visibility = Visibility.Collapsed;
        }

    }
}
