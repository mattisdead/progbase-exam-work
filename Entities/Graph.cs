using ScottPlot;
using System.Collections.Generic;
using System.Linq;
using ServiceLib;

namespace Entities
{
    public class Graph
    {
        public static void CreateGraph(List<Post> allPosts, List<Comment> allComments, string filePath)
        {
            int[] posts = new int[12];
            int[] comments = new int[12];
            for (int i = 0; i < allPosts.Count; i++)
            {
                if (allPosts[i].date.Month == 1)
                {
                    posts[0]++;
                }
                if (allPosts[i].date.Month == 2)
                {
                    posts[1]++;
                }
                if (allPosts[i].date.Month == 3)
                {
                    posts[2]++;
                }
                if (allPosts[i].date.Month == 4)
                {
                    posts[3]++;
                }
                if (allPosts[i].date.Month == 5)
                {
                    posts[4]++;
                }
                if (allPosts[i].date.Month == 6)
                {
                    posts[5]++;
                }
                if (allPosts[i].date.Month == 7)
                {
                    posts[6]++;
                }
                if (allPosts[i].date.Month == 8)
                {
                    posts[7]++;
                }
                if (allPosts[i].date.Month == 9)
                {
                    posts[8]++;
                }
                if (allPosts[i].date.Month == 10)
                {
                    posts[9]++;
                }
                if (allPosts[i].date.Month == 11)
                {
                    posts[10]++;
                }
                if (allPosts[i].date.Month == 12)
                {
                    posts[11]++;
                }
            }
            for (int i = 0; i < allComments.Count; i++)
            {
                if (allComments[i].date.Month == 1)
                {
                    comments[0]++;
                }
                if (allComments[i].date.Month == 2)
                {
                    comments[1]++;
                }
                if (allComments[i].date.Month == 3)
                {
                    comments[2]++;
                }
                if (allComments[i].date.Month == 4)
                {
                    comments[3]++;
                }
                if (allComments[i].date.Month == 5)
                {
                    comments[4]++;
                }
                if (allComments[i].date.Month == 6)
                {
                    comments[5]++;
                }
                if (allComments[i].date.Month == 7)
                {
                    comments[6]++;
                }
                if (allComments[i].date.Month == 8)
                {
                    comments[7]++;
                }
                if (allComments[i].date.Month == 9)
                {
                    comments[8]++;
                }
                if (allComments[i].date.Month == 10)
                {
                    comments[9]++;
                }
                if (allComments[i].date.Month == 11)
                {
                    comments[10]++;
                }
                if (allComments[i].date.Month == 12)
                {
                    comments[11]++;
                }
            }

            var plt = new ScottPlot.Plot(600, 400);

            string[] frequencies = { "Jan", "Feb", "Mar", "Apr", "May", "June", "July", "Aug", "Sep", "Oct", "Nov", "Dec" };

            double[] postsAmplitides = { posts[0], posts[1], posts[2], posts[3], posts[4], posts[5], posts[6], posts[7], posts[8], posts[9], posts[10], posts[11] };
            double[] commentsAmplitides = { comments[0], comments[1], comments[2], comments[3], comments[4], comments[5], comments[6], comments[7], comments[8], comments[9], comments[10], comments[11] };

            double[] positions = DataGen.Consecutive(frequencies.Length);
            plt.AddScatter(positions, postsAmplitides, System.Drawing.Color.Red, label: "Posts");
            plt.AddScatter(positions, commentsAmplitides, System.Drawing.Color.Purple, label: "Comments");

            string[] labels = frequencies.Select(x => x.ToString()).ToArray();
            plt.XAxis.ManualTickPositions(positions, labels);
            plt.XAxis.TickLabelStyle(rotation: 45);
            plt.XAxis.SetSizeLimit(min: 50);

            plt.YLabel("Number of posts/comments");
            plt.XLabel("Month");


            plt.SaveFig(filePath);
        }
    }
}