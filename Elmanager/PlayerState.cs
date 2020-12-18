namespace Elmanager
{
    internal struct PlayerState
    {
        public PlayerState(double globalBodyX, double globalBodyY, double leftWheelx, double leftWheely, double rightWheelx, double rightWheely, double leftWheelRotation, double rightWheelRotation, double headX, double headY, double bikeRotation, Direction direction, double armRotation)
        {
            this.globalBodyX = globalBodyX;
            this.globalBodyY = globalBodyY;
            this.leftWheelx = leftWheelx;
            this.leftWheely = leftWheely;
            this.rightWheelx = rightWheelx;
            this.rightWheely = rightWheely;
            this.leftWheelRotation = leftWheelRotation;
            this.rightWheelRotation = rightWheelRotation;
            this.headX = headX;
            this.headY = headY;
            this.bikeRotation = bikeRotation;
            this.direction = direction;
            this.armRotation = armRotation;
        }

        public double globalBodyX;
        public double globalBodyY;
        public double leftWheelx;
        public double leftWheely;
        public double rightWheelx;
        public double rightWheely;
        public double leftWheelRotation;
        public double rightWheelRotation;
        public double headX;
        public double headY;
        public double bikeRotation;
        public Direction direction;
        public double armRotation;
        public bool isright => direction == Direction.Right;
    }
}