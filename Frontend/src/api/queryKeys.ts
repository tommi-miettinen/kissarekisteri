export const QueryKeys = {
  USERS: "users",
  USER: "user",
  USER_BY_ID: (userId: string) => ["user", userId],
  CATS: "cats",
  CAT: "cat",
  CAT_BY_ID: (catId: number) => ["cat", catId],
};
