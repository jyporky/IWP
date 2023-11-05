using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PathInfo : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI pathName;
    [SerializeField] protected TextMeshProUGUI pathDescription;
    [SerializeField] protected Image pathImage;
    [SerializeField] Button interactPathButton;

    /// <summary>
    /// Update the path display according to the pathName, pathDescription and spriteImage provided.
    /// </summary>
    public void SetPath(string thePathName, string thePathDescription, Sprite thePathImage)
    {
        pathName.text = thePathName;
        pathDescription.text = thePathDescription;
        pathImage.sprite = thePathImage;
    }
    private void Start()
    {
        interactPathButton.onClick.AddListener(PathSelected);
    }

    /// <summary>
    /// Execute this function when a path has been chosen. This function will call the relevant path
    /// event from the ChamberManager accordingly.
    /// </summary>
    public virtual void PathSelected()
    {

    }
}