export const disablePullToRefresh = () => {
  document.body.style.overscrollBehavior = "none";
  document.documentElement.style.overscrollBehavior = "none";
};

export const enablePullToRefresh = () => {
  document.body.style.overscrollBehavior = "auto";
  document.documentElement.style.overscrollBehavior = "auto";
};
