public class ProblemDesctiptions
{
    public const char newLine = '\n';
    
    public const string Level1MoveUp =
        "Press \"I\" key to bring back the this information after closing it!\n" + 
        "The bottom cube on the screen is a target. You can select a target by clicking on it. " +
        "There are two types of targets. Test targets which accept plain classes with Solve() methods" + 
        "(there to check if you solved the programming challange correctly) and others to which you can " + 
        "attach MonoBehaviour scripts to alter their behaviour(the player character is this type of target too!)\n"+
        "Now you have to write a monobehaviour and apply it the current target(bottom cube) in a way that moves above the " + 
        "top cube!";
    public const string Level2 = "TODO:";
    public const string level3 = "TODO:";
    public const string Level4MCorners = 
        "There is a 2D matrix, you will be given its size, Y(rows) and X(cols)." +
        "You should return the coordinates of the four corner of the matrix"+
        "You should submit plain class with a method with the given singnature: \n public int[][] Solve(int y, int x)";
    public const string Level5MSelecAll = 
        "There is a 2D matrix, you will be given its size, Y(rows) and X(cols)." +
        "You should return the coordinates of all cells in the matrix"+
        "You should submit plain class with a method with the given singnature: \n public int[][] Solve(int y, int x)";
    public const string Level6MFindPath =
        "There is a 2D matrix, you will be given its size, Y(rows) and X(cols)." +
        "You are given the obstructed tile coordinates line int[](with y of the first tile being at index 0 " +
        "x of the first tile being at index 1 and so forth)" +
        "You should write an a method that returns a path to between two points outside of the matrix, that does not " +
        "pass through the obstructions and walk to the other end!\n" +
        "You should submit plain class with a method with the given singnature: \n" +
        "public int[][] Solve(int y, int x,int startY,int startX,int endY, int endX, int[] obstructions)";

    public const string Tutorial1StartMethod1 =
        "Welcome, here in Unity we will mostly used MonoBehaviour classes to write our code. For the most part they are like any other class with few specifics." +
        "For one they can anly exist as component of a GameObject and can not be initialized any other way"+
        "Also there are a couple of special methods that MonoBehaviours implements and right now we will look at Start()"+
        "A MonoBehaviour's Start() method is called then the object it is attached to is initialised. It is called only one!" + 
        "For this task you can only write inside the Start() method (but you can use whatever other methods are implemented insede the start method!)" + 
        "The goal of this challange is to make the target go throught all the 6 cubes";

    public const string Tutorial1StartMethod2 =
        "Again but harder";
}
    
