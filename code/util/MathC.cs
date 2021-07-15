using Sandbox;

public class MathC
{
    
    public static float Lerp(float numFrom, float numTo, float by){
        return numFrom * (1 - by) + numTo * by;
    }

}
