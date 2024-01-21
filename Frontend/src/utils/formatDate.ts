import moment from "moment";

export const formatDate = (start: string, end: string) => {
  const startDate = moment(start).format("l");
  const endDate = moment(end).format("l");
  const startTime = moment(start).format("LT");
  const endTime = moment(end).format("LT");
  return `${startDate} - ${endDate}, ${startTime} - ${endTime}`;
};
