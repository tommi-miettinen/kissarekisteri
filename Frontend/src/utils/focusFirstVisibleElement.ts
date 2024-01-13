export const focusFirstVisibleElement = (parentElem: HTMLElement) => {
  const focusableSelectors = 'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])';
  const focusableElements = [...parentElem.querySelectorAll(focusableSelectors)] || [];

  for (const elem of focusableElements) {
    if (isElementVisible(elem as HTMLElement)) {
      (elem as HTMLElement).focus();
      break;
    }
  }
};

const isElementVisible = (element: HTMLElement) => {
  const rect = element.getBoundingClientRect();
  const style = getComputedStyle(element);

  return (
    rect.width > 0 &&
    rect.height > 0 &&
    style.visibility !== "hidden" &&
    style.display !== "none" &&
    rect.top >= 0 &&
    rect.left >= 0 &&
    rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
    rect.right <= (window.innerWidth || document.documentElement.clientWidth)
  );
};
