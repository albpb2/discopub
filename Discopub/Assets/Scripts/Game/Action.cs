namespace Assets.Scripts.Game
{
    public struct Action
    {
        public string Name { get; set; }
        public string ControlType { get; set; }
        public string[] Values { get; set; }
        public int ActionPoints { get; set; }
    }
}
