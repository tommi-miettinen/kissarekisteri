export const handleFirstLoad = () => {
  window.addEventListener("beforeunload", () => {
    localStorage.removeItem("firstLoad");
  });

  if (!localStorage.firstLoad) {
    localStorage.firstLoad = true;
    history.pushState({}, "", window.location.href);
    history.pushState({}, "", window.location.href);
    window.history.forward();
  }
};
