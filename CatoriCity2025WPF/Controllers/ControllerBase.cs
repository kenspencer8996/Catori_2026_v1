using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Timers;

namespace CatoriCity2025WPF.Controllers
{
    public class ControllerBase
    {
        public List<string> _imageFileNames = new List<string>();
        Image _imageToAnimate;
        int _imageindex=0;

        private System.Timers.Timer _animation_timer;
        /// <summary>
        /// file search pattern to load images, e.g. "*.png"
        /// </summary>
        public string SearchPattern { get; set; } = "*.png";
        /// <summary>
        /// Images folder path
        /// </summary>
        public string ImagesFolder { get; set; } = "";

        public ControllerBase() 
        {
        }
        public void Startup(Image imageToAnimate)
        {
            _animation_timer = new System.Timers.Timer();
            _imageToAnimate = imageToAnimate;
            _animation_timer.Elapsed +=OnTimerTick;
            AnimationFrameInterval = AnimationFrameInterval;
            LoadImages();
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            if (_imageindex >= _imageFileNames.Count)
                _imageindex = 0;
            BitmapImage thisBitMap = new BitmapImage(new Uri(_imageFileNames[_imageindex]));
            thisBitMap.Freeze();

            if (thisBitMap != null && Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _imageToAnimate.Source = thisBitMap;
                });
            }
            _imageindex++; ;
        }


        public double AnimationFrameInterval
        {
            get => _animation_timer.Interval;
            set => _animation_timer.Interval = value;
        }
        public void StartAnimation()
        {
            _animation_timer.Start();
        }   
        public void StopAnimation()
        {
            _animation_timer.Stop();
        }

       
        private void LoadImages()
        {
            try
            {
                _imageFileNames = System.IO.Directory.GetFiles(ImagesFolder, SearchPattern).ToList();
                if (_imageFileNames.Count == 0)
                    throw new Exception("images not found in " + ImagesFolder);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        
    }
}
