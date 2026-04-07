using System;
using System.Windows.Forms;
using System.Threading;
using SDL2;

namespace EngineSimulator {
    public class SteeringWheel {

        private const int THROTTLE_AXIS = 2;
        private const int SECOND_AXIS = 1;

        private static bool sdlInit = false;

        private IntPtr joystick;
        private string name;
        
        public SteeringWheel() {
            joystick = SDL.SDL_JoystickOpen(0);
            name = SDL.SDL_JoystickName(joystick);
        }

        public static bool IsPresent() {
            if (!sdlInit) {
                SDL.SDL_Init(SDL.SDL_INIT_JOYSTICK);
                sdlInit = true;
            }
            
            return SDL.SDL_NumJoysticks() != 0;
        }

        public void Poll() {
            SDL.SDL_JoystickUpdate();
        }

        public double GetThrottle() {
            return AxisToDouble(SDL.SDL_JoystickGetAxis(joystick, THROTTLE_AXIS));
        }

        public double GetBrake() {
            bool enabled = !(Program.GetGearbox() is ManualGearbox) || Keyboard.IsKeyDown(Keys.LShiftKey);
            return enabled ? AxisToDouble(SDL.SDL_JoystickGetAxis(joystick, SECOND_AXIS)) : 0.0;
        }

        public double GetClutch() {
            bool enabled = (Program.GetGearbox() is ManualGearbox) && !Keyboard.IsKeyDown(Keys.LShiftKey);
            return enabled ? AxisToDouble(SDL.SDL_JoystickGetAxis(joystick, SECOND_AXIS)) : 0.0;
        }

        private double AxisToDouble(short axis) {
            return 1.0 - ((double)axis - short.MinValue) / (short.MaxValue - short.MinValue);
        }

        public string GetName() {
            return name;
        }

    }
}
