package Data;

import Database.Connect;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.List;

public class CompanyProfile implements Serializable{
	
	
	private static final long serialVersionUID = 1L;

	/* Data is saved in a company profile.
	 * along with relevant data (scores) */

	private String companyName;
	private List<Integer> companyScores = new ArrayList<Integer>();
	private List<String> companyArticles = new ArrayList<String>();

	public CompanyProfile(String companyName)
	{
		this.companyName = companyName;
	}

    public String CompanyName()
    {
        return companyName;
    }

    /* Adds a score to the company profile and
       in case it's valid we pass it to our DB
     */

	public String AddScore(int score, String title)
	{
		if(score != 0)
		{
			try
			{
				Connect.InsertScore(companyName, score, title);
			}
			catch(Exception e)
			{
				System.out.println("Failed to upload score to database");
			}

			companyScores.add(score);
			return "Succesfully added score of: " + score + " to profile: " + companyName;
		}
		else
			return "Did not save the score";
	}


    /* Add an article to this company profile */
	public void AddArticle(String fullTitle)
    {
        companyArticles.add(fullTitle);
    }

    /* This method returns true in case an article has already been
    added to the profile of this company, false if not
    */
    public boolean ArticleExists(String fullTitle)
    {
        if(companyArticles.contains(fullTitle))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /* Return all scores of this company profile in a
    readable format
     */
	public String AllScores()
	{
		String allScores = "";
		for(int i = 0; i < companyScores.size(); i++)
			allScores += companyScores.get(i) + "%|";

		return allScores;
	}


}
