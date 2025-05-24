
using System;
using System.Collections.Generic;
using System.Linq;
using minyee2913.Utils;
using UnityEngine;

public class Interaction : Singleton<Interaction>
{
    protected override bool UseDontDestroyOnLoad => false;
    [SerializeField]
    public InteractionCell cellPrefab;
    public Transform grids;
    public int selection;
    Dictionary<Action<PlayerController>, InteractionCell> actions = new();

    [SerializeField]
    PlayerController player;

    public void Set(Action<PlayerController> action, string comment)
    {
        if (!actions.ContainsKey(action))
        {
            InteractionCell cell = Instantiate(cellPrefab);
            cell.transform.SetParent(grids);

            cell.transform.localPosition = Vector3.zero;
            cell.transform.localScale = Vector3.one;
            cell.transform.localRotation = Quaternion.Euler(0, 0, 0);

            cell.comment.text = comment;


            actions[action] = cell;
        }
    }

    public void UnSet(Action<PlayerController> action)
    {
        if (actions.ContainsKey(action))
        {
            Destroy(actions[action].gameObject);
            actions.Remove(action);
        }
    }

    void Update()
    {
        if (actions.Count > 0)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            var arr = actions.ToArray();

            for (int i = 0; i < actions.Count; i++)
            {
                var pair = arr[i];

                if (i == selection)
                {
                    pair.Value.key.color = Color.white;
                }
                else
                {
                    pair.Value.key.color = Color.black;
                }
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("F");
                var selec = arr[selection];
                selec.Key?.Invoke(player);

                UnSet(selec.Key);
            }

            if (scroll > 0)
            {
                selection--;

                if (selection < 0)
                {
                    selection = 0;
                }
            }
            else if (scroll > 0)
            {
                selection++;

                if (selection >= actions.Count)
                {
                    selection = actions.Count - 1;
                }
            }
        }
        else
        {
            selection = 0;
        }
    }
}