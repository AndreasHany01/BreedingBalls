using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    public int nutritionValue = 40;

    public enum FoodType {Pizza, Sandwich, Meat, Cookie, Banana };

    public FoodType foodtype;
    public bool beingConsumed = false;




}
