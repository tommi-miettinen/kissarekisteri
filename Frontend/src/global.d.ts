interface CatShowEvent {
  id?: number;
  name: string;
  description: string;
  location: string;
  startDate: string;
  endDate: string;
  cats: { cat: Cat }[];
  photos?: { id: string; url: string }[];
  imageUrl: string;
}

interface Attendee {
  id: number;
  userId: string;
  user: User;
  catAttendees: { cat: Cat }[];
}

interface Permission {
  id: number;
  name: PermissionTypes;
}

interface Cat {
  sex: string;
  id: number;
  name: string;
  birthDate: string;
  breed: string;
  ownerId: string;
  breederId: string;
  imageUrl: string;
  photos: { id: string; url: string }[];
  results: CatShowResult[];
  parents: { parentCat: Cat }[];
  kittens: { childCat: Cat }[];
  owner: User | null;
  breeder: User | null;
}

interface CatShowResult {
  id: number;
  catShowId: number;
  catId: number;
  breed: string;
  place: number;
  catShow: CatShowEvent;
}

interface User {
  avatarUrl: string;
  id: string;
  email: string;
  givenName: string;
  displayName: string;
  surname: string;
  isBreeder: boolean;
  cats?: Cat[];
  permissions: Permission[];
  userRole: any;
  showPhoneNumber: boolean;
  showEmail: boolean;
  phoneNumber: string;
}

interface CatPayload {
  sex: string;
  name: string;
  birthDate: string;
  breed: string;
  fatherId?: number;
  motherId?: number;
}

interface EditCatPayload extends CatPayload {
  imageUrl?: string;
  id?: number;
}

interface CatShowResultPayload {
  catId: number;
  place: number;
  breed: string;
}

interface CatsGroupedByBreed {
  [breed: string]: Cat[];
}

interface CatBreed {
  id: number;
  name: string;
}

interface MsalConfig {
  authority: string;
  clientId: string;
  authorityDomain: string;
  redirectUri: string;
}

interface ApiResponse<T> {
  value: T[];
  isSuccess: boolean;
  data: T;
  errors: any[];
}

interface OdataResponse<T> {
  value: T[];
}

interface CatTransferRequest {
  id: number;
  cat: Cat;
  confirmerId: string;
  requester: User;
  requesterId: string;
  confirmed: boolean;
}

interface UserPayload {
  MailNickname: string;
  GivenName: string;
  Surname: string;
  DisplayName: string;
  Password: string;
  Email: string;
  Role: string;
}

interface SearchKey<T> {
  key: keyof T;
  exactMatch?: boolean;
  startsWith?: boolean;
}

type SearchKeys<T> = SearchKey<T>[];
