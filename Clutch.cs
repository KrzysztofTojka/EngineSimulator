using EngineSimulator;
using System;

public class Clutch {

    public const double DEAD_ZONE = 0.25;

    private Engine engine;
    private Gearbox gearbox;

    private double engagement = 0.0;

    private double maxTorque = 400;
    private double damping = 15.0;

    public Clutch(Engine engine, Gearbox gearbox, double maxTorque = 400) {
        this.engine = engine;
        this.gearbox = gearbox;
        this.maxTorque = maxTorque;
    }

    public void Update(double dt) {
        double engineOmega = engine.GetRPM() * 2 * Math.PI / 60;
        double gearboxOmega = gearbox.GetInputRPM() * 2 * Math.PI / 60;

        double slip = engineOmega - gearboxOmega;
        double torqueLimit = maxTorque * engagement;

        double torqueTransfer;
        if (engagement > 0.9 && Math.Abs(slip) < 35.0 && gearbox.GetCurrentGear() > 0) {
            double resistanceTorque = (gearbox.GetTotalResistance() * gearbox.GetWheelRadius()) / gearbox.GetTotalRatio();
            double netTorque = engine.GetBrakeTorque() - resistanceTorque;
            double netAccel = netTorque / (engine.GetInertia() + gearbox.GetCarInertia());
            torqueTransfer = engine.GetBrakeTorque() - (engine.GetInertia() * netAccel);
            double avgRpm = (engine.GetRPM() + gearbox.GetInputRPM()) / 2.0;
            engine.SetRPM(avgRpm);
            gearbox.SetInputRPM(avgRpm);
        } else {
            torqueTransfer = slip * damping * engagement;
            torqueTransfer = MathHelper.Clamp(torqueTransfer, -torqueLimit, torqueLimit);
        }

        engine.SetLoadTorque(torqueTransfer);
        gearbox.SetInputTorque(torqueTransfer);

        //Console.WriteLine($"RPM: {engine.GetRPM(),4:F0} | THR: {engine.GetThrottle():F2} | TQ_ENG: {engine.GetBrakeTorque(),6:F1} Nm | TRSF: {torqueTransfer,6:F1} Nm | SLIP: {slip,6:F2}");
    }

    public void SetPosition(double position) {
        position = Math.Max(0.0, (position - DEAD_ZONE) / (1.0 - DEAD_ZONE));
        engagement = Math.Pow(position, 2.5);
        engagement = MathHelper.Clamp(engagement, 0.0, 1.0);
    }

    public double GetEngangement() {
        return engagement;
    }

}