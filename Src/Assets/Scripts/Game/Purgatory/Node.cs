//using TMPro.Examples;
//using UnityEngine;

//public class Node : MonoBehaviour
//{
//    private Vector3 screenPoint;
//    private Vector3 offset;
//    private Camera myCamera;
//    private float? currentPositionYPrev = null;
//    private GameObject[] shadows = new GameObject[6]; 

//    private void Start()
//    {
//        this.myCamera = GameObject.Find("Camera").GetComponent<Camera>();

//        GameObject bottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        bottom.transform.localScale = new Vector3(1,0.1f,1);
//        bottom.SetActive(false);
//        shadows[0] = bottom;

//        GameObject top = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        top.transform.localScale = new Vector3(1, 0.1f, 1);
//        top.SetActive(false);
//        shadows[1] = top;

//        GameObject left = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        left.transform.localScale = new Vector3(0.1f, 1, 1);
//        left.SetActive(false);
//        shadows[2] = left;

//        GameObject right = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        right.transform.localScale = new Vector3(0.1f, 1, 1);
//        right.SetActive(false);
//        shadows[3] = right;

//        GameObject toUs = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        toUs.transform.localScale = new Vector3(1, 1, 0.1f);
//        toUs.SetActive(false);
//        shadows[4] = toUs;

//        GameObject away = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        away.transform.localScale = new Vector3(1, 1, 0.1f);
//        away.SetActive(false);
//        shadows[5] = away;
//    }

//    void OnMouseDown()
//    {
//        this.screenPoint = this.myCamera.WorldToScreenPoint(gameObject.transform.position);
//        this.offset = gameObject.transform.position - this.myCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
//        this.currentPositionYPrev = null;

//        foreach (var item in this.shadows)
//        {
//            item.SetActive(true);
//        }
//    }

//    void OnMouseDrag()
//    {
//        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
//        Vector3 curPosition = this.myCamera.ScreenToWorldPoint(curScreenPoint) + offset;
//        Vector3 temp = gameObject.transform.position;

//        if (Input.GetKey(KeyCode.LeftShift))
//        {
//            if (currentPositionYPrev != null && currentPositionYPrev != curPosition.y)
//            {
//                float yoff = curPosition.y - currentPositionYPrev.Value;
//                temp.z = temp.z + yoff;
//            }
//            currentPositionYPrev = curPosition.y;
//        }
//        else
//        {
//            temp.x = curPosition.x;
//            temp.y = curPosition.y;
//        }

//        this.shadows[0].transform.position = new Vector3(temp.x, -SpellcraftConstants.HalfSize, temp.z);
//        this.shadows[1].transform.position = new Vector3(temp.x, +SpellcraftConstants.HalfSize, temp.z);

//        this.shadows[2].transform.position = new Vector3(-SpellcraftConstants.HalfSize, temp.y, temp.z);
//        this.shadows[3].transform.position = new Vector3(+SpellcraftConstants.HalfSize, temp.y, temp.z);

//        this.shadows[4].transform.position = new Vector3(temp.x, temp.y, -SpellcraftConstants.HalfSize);
//        this.shadows[5].transform.position = new Vector3(temp.x, temp.y, +SpellcraftConstants.HalfSize);

//        gameObject.transform.position = temp;
//    }

//    private void OnMouseUp()
//    {
//        foreach (var item in this.shadows)
//        {
//            item.SetActive(false);
//        }
//    }
//}
