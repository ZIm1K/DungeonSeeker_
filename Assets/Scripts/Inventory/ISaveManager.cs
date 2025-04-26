using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveManager
{
    public void Save<T>(T data);

    public T Load<T>();

}
