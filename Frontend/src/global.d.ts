interface CatShowEvent {
  id?: number;
  name: string;
  description: string;
  location: string;
  startDate: string;
  endDate: string;
  attendees?: User[];
}

interface Cat {
  id: number;
  name: string;
  birthDate: Date;
  breed: string;
  ownerId: string;
  breederId: string;
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
}
