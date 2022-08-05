public class MathC
{
	public static float Lerp(float numFrom, float numTo, float by){
        return numFrom * (1 - by) + numTo * by;
    }

    public static Vector3 Lerp(Vector3 vecFrom, Vector3 vecTo, float by){
        return new Vector3(
            Lerp(vecFrom.x, vecTo.x, by),
            Lerp(vecFrom.y, vecTo.y, by),
            Lerp(vecFrom.z, vecTo.z, by)
        );
    }

    public static float Map(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        if(outputMin > outputMax)
        {
            var _temp = inputMin;
            inputMin = inputMax;
            inputMax = _temp;
        }
        var  _val = ((value - inputMin) / (inputMax - inputMin));
        if(outputMin > outputMax)
        {
            return outputMin + ((1f-_val) * (outputMax - outputMin));
        }
        else
        {
            return outputMin + (_val * (outputMax - outputMin));  
        }
    }

}
