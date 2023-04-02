using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace HonkBusterGame
{
    public partial class ScoreBar : Border
    {
        #region Fields

        private readonly TextBlock _textBlock;
        private double _score;

        #endregion

        #region Ctor

        public ScoreBar()
        {
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;

            _textBlock = new TextBlock() { FontSize = 30, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.White) };

            this.Child = _textBlock;
            GainScore(0);
        }

        #endregion

        #region Methods

        public void Reset()
        {
            _score = 0;
            _textBlock.Text = _score.ToString("0000");
        }

        public void GainScore(int score)
        {
            _score += score;
            _textBlock.Text = _score.ToString("0000");
        }

        public void LooseScore(double score)
        {
            if (_score > 1)
            {
                _score -= score;
                _textBlock.Text = _score.ToString("0000");
            }
        }

        public double GetScore()
        {
            return _score;
        }

        #endregion
    }
}
