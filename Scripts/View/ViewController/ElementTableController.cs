using UnityEngine;
using UnityEngine.UI;

namespace Xsolla
{
    public class ElementTableController: MonoBehaviour
    {
        public Text _title;
        public GameObject _container;

        public void InitScreen(XsollaFormElement pElem)
        {
            _title.text = pElem.GetTableOptions()._head[0];

            foreach (string item in pElem.GetTableOptions()._body)
            {
                GameObject itemtable = Instantiate(Resources.Load("Prefabs/SimpleView/_PaymentFormElements/ContainerTableItem")) as GameObject;
                itemtable.GetComponentInChildren<Text>().text = item;
                itemtable.transform.SetParent(_container.transform);
            }

        }
    }
}