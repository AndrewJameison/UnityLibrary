using UnityEngine;

public static class Global
{
    public void Pause()
    {
        
    }

    public void WaitUntilNotNull(object obj)
    {
        while (obj == null)
        {
            yield return null;
        }

        // TODO: pass in function to call after object is not null
    }
}
