import { Photo } from './photo';

export interface User {
    id: number;
    userName: string;
    knownas: string;
    age: number;
    gender: string;
    created: Date;
    photoUrl: string;
    city: string;
    country: string;
    interests?: string;
    lastActive?: Date;
    introduction?: string;
    lookingFor?: string;
    photos?: Photo[];
}
