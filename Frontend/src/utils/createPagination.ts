export const createPagination = (currentPage: number, totalPages: number, sliceSize: number) => {
  const pageItems = Array.from({ length: totalPages }, (_, i) => i);

  const displayOnLeft = Math.floor(sliceSize / 2);
  const displayOnRight = Math.ceil(sliceSize / 2);

  const middleItems = pageItems.slice(currentPage - displayOnLeft, currentPage + displayOnRight);
  const leftItems = pageItems.slice(2, 2 + sliceSize);
  const rightItems = pageItems.slice(-sliceSize);

  let itemsToDisplay: string | number[] = [];
  let leftEllipsis;
  let rightEllipsis;

  if (currentPage < leftItems[leftItems.length - 1]) itemsToDisplay = leftItems;

  if (currentPage > rightItems[0]) itemsToDisplay = rightItems;

  if (currentPage >= leftItems[leftItems.length - 1] && currentPage <= rightItems[0]) {
    itemsToDisplay = middleItems;
  }

  if (itemsToDisplay[0] > 2) leftEllipsis = "...";

  if (itemsToDisplay[itemsToDisplay.length - 1] < totalPages - 1) {
    rightEllipsis = "...";
  }

  return [1, leftEllipsis, ...itemsToDisplay, rightEllipsis, totalPages].filter(Boolean);
};
