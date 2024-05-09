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
        private Random rnd = new Random();
        private List<string> audioNames = new List<string>() {"MainThemeSoundTrack", "PeaceFulJourneyTheme"};
        private GameAudioPlayer()
        {
            SoundOptionIcon = SoundOff;
        }


        private void SelectAudio()
        {
            try
            {
                int index = rnd.Next(audioNames.Count);
                soundPlayer.SoundLocation = $"assets/Audio/Themes/{audioNames[index]}.wav";
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
                SelectAudio();
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
