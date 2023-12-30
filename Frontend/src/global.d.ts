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
  birthDate: Date;
  breed: string;
  ownerId: string;
  breederId: string;
  imageUrl: string;
  photos: { id: string; url: string }[];
  results: CatShowResult[];
  catParents: Cat[];
  kittens: Cat[];
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
}

interface CatPayload {
  sex: string;
  name: string;
  birthDate: Date | null;
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
