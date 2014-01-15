using System.Linq;
using System.Windows.Input;

namespace KaVE.Utils.IO
{
    public class KeyBinding
    {
        private readonly Key[][] _keyCombinations;

        public KeyBinding(Key[][] keyCombinations)
        {
            _keyCombinations = keyCombinations;
        }

        public Key[][] Combinations
        {
            get
            {
                return _keyCombinations;
            }
        }

        public bool IsPressed()
        {
            return _keyCombinations.Last().All(Keyboard.IsKeyDown);
        }
    }
}
