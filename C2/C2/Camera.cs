using C2;

class Camera : Equipment
{
    private int Resolution {set; get;}
    private bool hasStapilization {set; get;}

    public Camera(string producer, string name, int resolution, bool hasStapilization) : base(producer, name)
    {
        Resolution = resolution;
        this.hasStapilization = hasStapilization;
    }
}