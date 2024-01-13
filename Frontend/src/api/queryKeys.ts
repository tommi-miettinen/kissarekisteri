export const QueryKeys = {
  CAT_SHOWS: ["catShows"],
  CAT_SHOW_BY_ID: (catShowId: number) => ["catShow", catShowId],
  USERS_CATS: ["usersCats"],
  USERS: ["users"],
  USER: ["user"],
  USER_BY_ID: (userId: string) => ["user", userId],
  CATS: ["cats"],
  CAT: ["cat"],
  CAT_BY_ID: (catId: number) => ["cat", catId],
  CONFIRMATION_REQUESTS: ["confirmationRequests"],
  ROLES: ["roles"],
};
