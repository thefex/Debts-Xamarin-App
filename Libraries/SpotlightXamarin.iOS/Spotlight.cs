using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace SpotlightXamarin.IOS
{
    public class Spotlight
    {
        public delegate void OnSpotlightStartedHandler();
        public OnSpotlightStartedHandler OnSpotlightStarted;

        public delegate void OnSpotlightEndedHandler();
        public OnSpotlightEndedHandler OnSpotlightEnded;

        public delegate void OnTargetStartedHandler(Target target);
        public OnTargetStartedHandler OnTargetStarted;

        public delegate void OnTargetEndedHandler(Target target);
        public OnTargetEndedHandler OnTargetEnded;

        private SpotlightView SpotlightView;
        private UIViewController ParentController;
        private List<Target> Targets;
        private long Duration;

        private nfloat StartStoplightAnimationDuration = 0.5f;
        private nfloat FinishStoplightAnimationDuration = 0.5f;

        public Spotlight(UIViewController controller, List<Target> targets, long duration)
        {
            ParentController = controller;
            Targets = targets;
            Duration = duration;
        }

        public void Start()
        {
            if (ParentController == null)
            {
                throw new Exception("Spotlight: controller is null");
            }

            SpotlightView = new SpotlightView(ParentController, ParentController.View.Bounds);

            ParentController.Add(SpotlightView);

            SpotlightView.OnTargetClosed += () =>
            {
                if (Targets.Count > 0)
                {
                    Target target = Targets[0];

                    OnTargetEnded?.Invoke(target);

                    Targets.Remove(target);

                    if (Targets.Count > 0)
                    {
                        StartTarget();
                    }
                    else
                    {
                        FinishSpotlight();
                    }
                }
            };

            SpotlightView.OnTargetClicked += () =>
            {
                FinishTarget();
            };

            StartSpotlight();
        }

        private void StartTarget()
        {
            if (Targets != null && Targets.Count > 0)
            {
                Target target = Targets[0];

                foreach(var view in SpotlightView.Subviews)
                {
                    view.RemoveFromSuperview();
                }

                SpotlightView.Add(target.View);

                SpotlightView.TurnUp(target.Point.X, target.Point.Y, target.Radius, Duration);

                OnTargetStarted?.Invoke(target);
            }
        }

        private void StartSpotlight()
        {
            SpotlightView.Alpha = 0;
            UIView.Animate(StartStoplightAnimationDuration, () =>
             {
                 SpotlightView.Alpha = 1;
                 OnSpotlightStarted?.Invoke();
             }, () =>
             {
                 StartTarget();
             });
        }

        private void FinishTarget()
        {
            if (Targets != null && Targets.Count > 0)
            {
                Target target = Targets[0];
                SpotlightView.TurnDown(target.Radius, Duration);
            }
        }

        private void FinishSpotlight()
        {
            SpotlightView.Alpha = 1;
            UIView.Animate(FinishStoplightAnimationDuration, () =>
            {
                SpotlightView.Alpha = 0;

            }, () =>
            {
                SpotlightView.RemoveFromSuperview();
                OnSpotlightEnded?.Invoke();
            });
        }
    }

    public class SpotlightBuilder
    {
        private UIViewController ParentController;
        private List<Target> Targets;
        private long Duration = 1000;

        public SpotlightBuilder(UIViewController controller)
        {
            ParentController = controller;
        }

        public SpotlightBuilder SetTargets(List<Target> targets)
        {
            Targets = targets;
            return this;
        }

        public SpotlightBuilder SetTargets(params Target[] targets)
        {
            Targets = targets.ToList();
            return this;
        }

        public SpotlightBuilder SetDuration(long duration)
        {
            Duration = duration;
            return this;
        }

        public Spotlight Start()
        {
            Spotlight spotlight = new Spotlight(ParentController, Targets, Duration);
            spotlight.Start();

            return spotlight;
        }
    }
}
