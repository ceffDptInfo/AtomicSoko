using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace atomicSoko
{
    class GameAudioPlayer
    {

        public ImageBrush SoundOptionIcon { get; set; }
        public event EventHandler? SoundOnOffUpdated;

        static public GameAudioPlayer Instance = new GameAudioPlayer();

        private bool isMuted = true;
        private SoundPlayer soundPlayer = new SoundPlayer();
        private ImageBrush SoundOn = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/icons/SoundOn.png")));
        private ImageBrush SoundOff = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/assets/images/icons/SoundOff.png")));

        private GameAudioPlayer()
        {
            SoundOptionIcon = SoundOff;
        }


        public void SelectAudio()
        {
            try
            {
                soundPlayer.SoundLocation = "assets/Audio/Themes/ThemeSoundTrack.wav";
                soundPlayer.Load();
            }catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public void ToggleSound()
        {
            isMuted = !isMuted;
            if (isMuted)
            {
                StopCurrentAudio();
                SoundOptionIcon = SoundOff;

            }
            else
            {
                StartCurrentAudio();
                SoundOptionIcon = SoundOn;
            }
            SoundOnOffUpdated?.Invoke(null, EventArgs.Empty);
        }

        private void StopCurrentAudio()
        {
            soundPlayer.Stop();
        }
        private void StartCurrentAudio()
        {
            soundPlayer.PlayLooping();
        }
    }
}
