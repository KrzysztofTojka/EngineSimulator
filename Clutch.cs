using EngineSimulator;
using System;

public class Clutch {

    private Engine engine;
    private Gearbox gearbox;

    private double engagement = 0.0;

    private double maxTorque = 500;
    private double stiffness = 2;

    public Clutch(Engine engine) {
        this.engine = engine;
    }

    public void Update(double dt) {
        double engineOmega = engine.GetRPM() * 2 * Math.PI / 60;
        double gearboxOmega = gearbox.GetInputRPM() * 2 * Math.PI / 60;

        double slip = engineOmega - gearboxOmega;
        double torqueLimit = maxTorque * engagement;

        double damping = 15.0;
        double torqueTransfer = slip * damping * engagement;

        torqueTransfer = MathHelper.Clamp(torqueTransfer, -torqueLimit, torqueLimit);

        engine.SetLoadTorque(torqueTransfer);
        //Console.WriteLine($"Clutch Slip: {slip:F2} rad/s, Torque Transfer: {torqueTransfer:F2} Nm");
        gearbox.SetInputTorque(torqueTransfer);
    }

    public void SetEngagement(double value) {
        engagement = MathHelper.Clamp(value, 0, 1);
    }

    public double GetEngangement() {
        return engagement;
    }

    public void SetGearbox(Gearbox gearbox) {
        this.gearbox = gearbox;
    }
}