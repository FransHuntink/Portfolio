package Algorithm;

import java.io.IOException;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class TextAnalyzer {
	
	/* This class will analyze a body of content
	 * by comparing it to a word dataset. It also
	 * determines which company a body of content is
	 * about based on a company dataset. 
	 */
	
	private List<String> filterWords; /* The indicator words this analyzer will look for */
	private List<String> companyWords; /* The companies this analyzer will look for */
	private Map<String, Integer> possibleCompanies = new HashMap<String, Integer>(); /* HashMap of all possible companies */
	private boolean returnDebugs = false;
	private int lastScore = 0;
	
	public TextAnalyzer(List<String> filterWords, List<String> companiesToCheck) 
	{
		// create a hashmap for possible companies
		this.companyWords = companiesToCheck;
		this.filterWords = filterWords;
		
		SetupHashmap();
	}

	/*
	 * Analyzes a body of content, not intended to be used for small-sized texts
	 * like titles. Use ReturnCompany instead */
	
	public String AnalyzeList(List<String> articleText) {

        /* Variables to save text analyzer results in */

		int matchingWords = 0;
		int badWordCount = this.filterWords.size();
		double wordCount = articleText.size();
		double matchingPercentage = 0;
		String result = "";

		/* Compare the input strings to the comparison dataset */
		for (int i = 0; i < wordCount; i++) {
			if (this.filterWords.contains(articleText.get(i)))
			{
				matchingWords++;

				if(returnDebugs)
					result += "ANALYZER: Bad word #" + matchingWords + " detected in text: " + articleText.get(i);
			}
		}
		
		/* Calculate the matching percentage between matching
		 * words & the total amount of words */

		matchingPercentage = Math.round((matchingWords / wordCount) * 100);


		lastScore = (int)matchingPercentage;

		/* Save the data in a result string */
		result += String.format("%-30s %s\n", "State:", "Succesfull, data output below"); 
		result += String.format("%-30s %s\n", "Input wordcount:", wordCount); 
		result += String.format("%-30s %s\n", "Comparison list wordcount:", badWordCount);
		result += String.format("%-30s %s\n", "Matching percetange:", matchingPercentage + "%"); 
		
		
		return result;
	}


    public int ReturnLastScore()
	{
		return this.lastScore;
	}


    /* Attempts to return the company an body of
     * content is by comparing text to an existing
     * company dataset */

    public String ReturnCompany(List<String> content)
	{
		int currentMax = 0, maxIndex = 0;
		String result = "";

		/* We call this to reset all results 
		 * in the hash map after
		 * scanning a body of content */

		SetupHashmap();
		
		/* Compares the content to a set of companie names */
		
		for (int i = 0; i < content.size(); i++) 
		{
			/* In a word matches the company names, we add this to that companies' hashmap */
			if (companyWords.contains(content.get(i))) // we found a word that might be a company
			{
				/* The new entry amount this company will get */
				int newCount = possibleCompanies.get(content.get(i)) + 1;
				possibleCompanies.put(content.get(i), newCount);
				
				if(newCount > currentMax)
				{
					currentMax = newCount;
					maxIndex = i;
				}
			}
		}

		/* currentMax is the index of the currently
		   most likely company in the hashmap */

		if(currentMax != 0)
			result += content.get(maxIndex);
		else
			return null;
		
		return result;
	}

	

	/* The hashmap used to determine to company
	is reset using this method */
	private void SetupHashmap() 
	{
		List<String> hashmapData = this.companyWords;
		for (int i = 0; i < hashmapData.size(); i++)
		{
			possibleCompanies.put(hashmapData.get(i), 0);
		}
	}
	


}
