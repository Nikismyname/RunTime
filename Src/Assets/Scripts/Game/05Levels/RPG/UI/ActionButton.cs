using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    public int ID;
    public string CubeName;
    private bool isRpg = false;
    private LevelMainRPG rpg;

    private void Start()
    {
        this.ID = int.Parse(name.ToCharArray().Last().ToString());
        StartCoroutine(Setup());
    }

    public IEnumerator Setup()
    {
        yield return null;
        yield return null;

        if (ReferenceBuffer.Instance.LevelManager.levelMono.GetType() == typeof(LevelMainRPG))
        {
            this.isRpg = true;
        }
        else
        {
            this.isRpg = false;
        }

        if (this.isRpg)
        {
            this.GetComponent<Button>().onClick.AddListener(this.OnClick);
            this.rpg = (LevelMainRPG)ReferenceBuffer.Instance.LevelManager.levelMono;
            var mappings = ActionKeyPersistance.GetKeyCubeMapping();

            var myMap = mappings.SingleOrDefault(x => x.KeyId == this.ID);

            if (myMap != null)
            {
                this.CubeName = myMap.CubeName;
                this.transform.Find("Text").GetComponent<Text>().text = this.CubeName;
            }
        }
        else
        {
            //Debug.Log("This is not rpg, action buttons do nothing!");
        }
    }

    public void OnClick()
    {
        ///Unassigned
        if(CubeName == null)
        {

        }
        ///Assigned
        else
        {
            
        }
    }
}

