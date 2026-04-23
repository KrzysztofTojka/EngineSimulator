using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EngineSimulator {
    public class Keyboard {

        private Dictionary<Keys, bool> keyStates = new Dictionary<Keys, bool>();

        public void Update() {
            foreach (var key in keyStates.Keys.ToList()) {
                if (!IsKeyDown(key)) {
                    keyStates[key] = false;
                }
            }
        }

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey);

        public bool IsKeyDown(Keys key) {
            return (GetAsyncKeyState(key) & 0x8000) != 0;
        }

        public bool WasKeyPressed(Keys key) {
            if (IsKeyDown(key) && (!keyStates.ContainsKey(key) || !keyStates[key])) {
                keyStates[key] = true;
                return true;
            }
            return false;
        }

    }
}
