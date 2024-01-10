interface CatShowEvent {
  id?: number;
  name: string;
  description: string;
  location: string;
  startDate: string;
  endDate: string;
  attendees?: Attendee[];
  photos?: { id: string; url: string }[];
}

interface Attendee {
  id: number;
  userId: string;
  user: User;
  catAttendees: { cat: Cat }[];
}

interface Permission {
  id: number;
  name: string;
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
  owner: User;
  breeder: User;
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
  isSuccess: boolean;
  data: T;
  errors: any[];
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
