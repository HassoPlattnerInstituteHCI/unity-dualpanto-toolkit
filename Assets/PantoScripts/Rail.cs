namespace DualPantoFramework
{
    public class Rail : PantoBoxCollider 
    {
        public Rail()
        {
            isPassable = true;
        }

        public override void CreateObstacle()
        {
            CreateRail();
        }
    }
}