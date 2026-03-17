using EngineSimulator;
using System;

public class Clutch {

    private Engine engine;
    private Gearbox gearbox;

    private double engagement = 0.0;

    private double maxTorque = 500;
    private double stiffness = 2;
    private double damping = 15.0;

    public Clutch(Engine engine) {
        this.engine = engine;
    }

    public void Update(double dt) {
        double engineOmega = engine.GetRPM() * 2 * Math.PI / 60;
        double gearboxOmega = gearbox.GetInputRPM() * 2 * Math.PI / 60;

        double slip = engineOmega - gearboxOmega;
        double torqueLimit = maxTorque * engagement;

        double torqueTransfer;
        if (engagement > 0.9 && Math.Abs(slip) < 30.0 && gearbox.GetCurrentGear() > 0) {
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
        engine.UpdateRpm(dt);

        gearbox.SetInputTorque(torqueTransfer);

        //Console.WriteLine($"RPM: {engine.GetRPM(),4:F0} | THR: {engine.GetThrottle():F2} | TQ_ENG: {engine.GetBrakeTorque(),6:F1} Nm | TRSF: {torqueTransfer,6:F1} Nm | SLIP: {slip,6:F2}");
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