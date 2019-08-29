import { IActivityFormValues } from './activity'

export interface IActivity {
  id: string,
  title: string,
  description: string,
  category: string,
  date: Date,
  city: string,
  venue: string
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