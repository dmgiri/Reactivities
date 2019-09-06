import { IActivityFormValues } from './activity'

export interface IActivity {
  id: string,
  title: string,
  description: string,
  category: string,
  date: Date,
  city: string,
  venue: string,
  isGoing: boolean,
  isHost: boolean,
  attendees: IAttendee[],
  comments: IComment[]
}

export interface IAttendee {
  username: string, 
  displayName: string;
  image: string,
  isHost: boolean,
  following?: boolean
}

export interface IComment {
  id: string,
  body: string,
  username: string,
  displayName: string,
  image: string,
  createdAt: Date
}

export interface IActivityFormValues extends Partial<IActivity> { time?: Date }


export class ActivityFormValues implements IActivityFormValues {

  id?: string = undefined;
  title: string = '';
  description: string = '';
  category: string = '';
  date?: Date = undefined;
  time?: Date = undefined;
  city: string = '';
  venue: string = '';

  constructor(init?: IActivityFormValues) {
    if (init && init.date) { init.time = init.date }
    Object.assign(this, init)
  }
}

// notes:
// Object.assign(this, init) means that all the properties of init are assigned to the corresponding properties of a new instance of the class (this).

// isGoing: is the currently logged in user going to this activity?
// isHost:  is the currently logged in user hosting this event?