using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace VSX.Utilities
{
    /// <summary>
    /// This class provides a fast lookup for converting float distances to strings, because the .ToString() method
    /// creates garbage and impacts performance.
    /// </summary>
	public class DistanceStringLookup : MonoBehaviour
    {
	
		[Tooltip("The maximum displayable range in metres.")]
		[SerializeField]
		protected int maxDistanceInMetres = 10000;

        [Tooltip("The maximum displayable range in metres.")]
        [SerializeField]
		protected string outOfRangeLabel = "OutOfRange";

        [Tooltip("The resolution at which to display kilometres.")]
        [SerializeField]
		protected float kilometreResolution = 0.1f;
		protected int numEntriesPerKilometre;

        [Tooltip("The threshold (in metres) at which to start displaying in kilometres.")]
        [SerializeField]
		protected int thresholdKM = 200;
	
		protected List<string> s_DistanceLookupKilometres = new List<string>();
		protected List<string> s_DistanceLookupMetres = new List<string>();

        public static DistanceStringLookup Instance;


		
		// Called only on the editor when script is loaded or values are changed
		protected virtual void OnValidate()
		{	
			// Clamp the kilometre resolution from 1 metre to 1 kilometre
			kilometreResolution = Mathf.Clamp(kilometreResolution, 0.001f, 1f);

            // Make sure the resolution gives a whole number
            kilometreResolution = 1f / (Mathf.RoundToInt(1f / kilometreResolution));
		}
	

		protected virtual void Awake()
		{
	
            // Assign singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

			// Clear the lookup lists
			s_DistanceLookupKilometres.Clear();

			s_DistanceLookupMetres.Clear();

			int numKilometres = maxDistanceInMetres / 1000;

            numEntriesPerKilometre = Mathf.RoundToInt(1 / kilometreResolution);
			
			// Fill the kilometre list
			for (int i = 0; i < numKilometres; ++i)
			{
				for (int j = 0; j < numEntriesPerKilometre; ++j)
				{
					float decimalPart = j * kilometreResolution;
					float num = i + decimalPart;
					string result = num.ToString("F1") + " KM";
					s_DistanceLookupKilometres.Add (result);
				}
			}
	
			// Fill the metre list for higher resolution distance info at close proximity
			for (int i = 0; i < thresholdKM; ++i){
				string result = i.ToString() + " M";
				s_DistanceLookupMetres.Add(result);
			}
		}


        /// <summary>
        /// Get a string that displays a distance.
        /// </summary>
        /// <param name="distanceInMetres">The distance.</param>
        /// <returns>The distance value as a string, either in kilometres or metres depending on the threshold.</returns>
        public virtual string Lookup(float distanceInMetres)
		{
	
			// If less than threshold, return the distance in metre format
			if (distanceInMetres < thresholdKM)
			{
				int val = (int)distanceInMetres;
				return s_DistanceLookupMetres.Count > val ? s_DistanceLookupMetres[(int)distanceInMetres] : outOfRangeLabel;
			}
			else
			{
	
				int wholeIndex = (int)(distanceInMetres / 1000f);
				int fractionIndex = (int)(((distanceInMetres / 1000f) - wholeIndex) / kilometreResolution);
					
				int index = wholeIndex * numEntriesPerKilometre + fractionIndex;

				if (index < s_DistanceLookupKilometres.Count)
				{
					return (s_DistanceLookupKilometres[index]);
				}
				else
				{
					return outOfRangeLabel;
				}
			}
		}
	}
}