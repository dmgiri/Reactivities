import { IUser } from '../../models/user';
import { IActivity, IAttendee } from '../../models/activity';

export const combineDateAndTime = (date: Date, time: Date) => {
  const dateString = date.toISOString().split('T')[0]
  const timeString = date.toISOString().split('T')[1]
  return new Date(dateString + ' ' + timeString)
}

export const setActivityProps = (activity: IActivity, user: IUser) => {
  activity.date = new Date(activity.date)
  activity.isGoing = activity.attendees.some(a => a.username === user.username)
  activity.isHost = activity.attendees.some(a => a.username === user.username && a.isHost === true)
  return activity
}

export const createAttendee = (user: IUser): IAttendee => {
  return { displayName: user.displayName, username: user.username, isHost: false, image: user.image! }
}