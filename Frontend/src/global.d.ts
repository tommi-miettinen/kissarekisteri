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
  catAttendees: Cat[];
}

interface Permission {
  id: number;
  name: string;
}

interface Cat {
  id: number;
  name: string;
  birthDate: Date;
  breed: string;
  ownerId: string;
  breederId: string;
  imageUrl: string;
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
  name: string;
  birthDate: Date;
  breed: string;
}

interface EditCatPayload extends CatPayload {
  imageUrl?: string;
  id: number;
}
