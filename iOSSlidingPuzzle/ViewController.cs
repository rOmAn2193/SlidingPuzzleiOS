using System;
using System.Collections;
using CoreGraphics;
using Foundation;
using UIKit;

namespace iOSSlidingPuzzle
{
    public partial class ViewController : UIViewController
    {
        #region vars
        float gameViewWidth;
        float tileWidth;


        ArrayList tilesArr;
        ArrayList centersArr;


        CGPoint emptySpot;

        int curTime = 0;
        NSTimer gameTimer;

        #endregion



        protected ViewController (IntPtr handle) : base (handle) { }

        public override void ViewDidLoad ( )
        {
            base.ViewDidLoad ( );
        }


        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);

            gameView.LayoutIfNeeded ( );
            gameViewWidth = (float)gameView.Frame.Size.Width;

            makeTiles ( );



            ResetButton_TouchUpInside (null);
        }


        private void makeTiles ( )
        {
            tilesArr = new ArrayList ( );
            centersArr = new ArrayList ( );


            tileWidth = gameViewWidth / 4;


            float xCen = tileWidth / 2;
            float yCen = tileWidth / 2;
            int counter = 0;

            for (int h = 0; h < 4; h++)
            {
                for (int i = 0; i < 4; i++)
                {
                    UILabel textTile = new UILabel ( );
                    CGRect tileFrame = new CGRect (0, 0, tileWidth - 4, tileWidth - 4);

                    textTile.Frame = tileFrame;

                    CGPoint tileCen = new CGPoint (xCen, yCen);



                    textTile.Center = tileCen;


                    textTile.BackgroundColor = UIColor.Green;

                    textTile.Text = counter.ToString ( );
                    textTile.TextAlignment = UITextAlignment.Center;
                    textTile.Font = UIFont.SystemFontOfSize (25);






                    textTile.UserInteractionEnabled = true;


                    // add the cur center and cur tile to the arrays
                    tilesArr.Add (textTile);
                    centersArr.Add (tileCen);



                    gameView.AddSubview (textTile);


                    xCen = xCen + tileWidth;
                    counter = counter + 1;
                }

                xCen = tileWidth / 2;
                yCen = yCen + tileWidth;
            }



            // 0    1   2   3 -> 15 {16}

            UILabel lastLabel = (UILabel)tilesArr[15];
            lastLabel.RemoveFromSuperview ( );
            tilesArr.RemoveAt (15);


        }

        private void randomizeMethod ( )
        {
            ArrayList tempCentersArr = new ArrayList (centersArr);



            Random myRandom = new Random ( );


            foreach (UILabel anyLabel in tilesArr)
            {
                int randomIndex = myRandom.Next (0, tempCentersArr.Count);

                anyLabel.Center = (CGPoint)tempCentersArr[randomIndex];


                tempCentersArr.RemoveAt (randomIndex);
            }

            emptySpot = (CGPoint)tempCentersArr[0];
        }


        partial void ResetButton_TouchUpInside (UIButton sender)
        {
            randomizeMethod ( );


            curTime = 0;
            timerLabel.Text = "0\' : 0\"";


            if (gameTimer != null)
                gameTimer.Invalidate ( );

            gameTimer = NSTimer.CreateRepeatingScheduledTimer (TimeSpan.FromSeconds (1.0),
                                                               delegate
                                                                {
                                                                    timerMethod ( );
                                                                });
        }

        private void timerMethod ( )
        {
            curTime = curTime + 1; // 125 - 120  = 5

            int minuteVal = curTime / 60; // 2
            int secondVal = curTime % 60; // 5


            timerLabel.Text = minuteVal.ToString ( ) + "\' : " + secondVal.ToString ( ) + "\"";
        }



        public override void TouchesEnded (NSSet touches, UIEvent evt)
        {
            base.TouchesEnded (touches, evt);

            int touchesCount = (int)touches.Count;

            if (touchesCount == 1)
            {
                UITouch myTouch = (UITouch)touches.AnyObject;

                UIView touchedView = myTouch.View;

                if (tilesArr.Contains (touchedView))
                {
                    CGPoint thisCenter = touchedView.Center;
                    float horDist = (float)Math.Pow (thisCenter.X - emptySpot.X, 2);
                    float verDist = (float)Math.Pow (thisCenter.Y - emptySpot.Y, 2);


                    float distance = (float)Math.Sqrt (horDist + verDist);

                    if (distance == tileWidth)
                    {
                        UIView.Animate (.15f,
                                        ( ) => // animation
                                            {
                                                touchedView.Center = emptySpot;
                                            },
                                        ( ) => // completion
                                            {
                                                emptySpot = thisCenter;
                                            });
                                            }
                }
            }
        }
    }
}
