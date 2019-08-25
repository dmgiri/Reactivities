import React, { Fragment, useState, useEffect, SyntheticEvent } from "react"
import { Container } from "semantic-ui-react"
import { IActivity } from "../models/activity"
import NavBar from "../../features/nav/NavBar"
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard'
import agent from '../api/agent'
import LoaderComponent from './LoaderComponent'


const App: React.FC = () => {

  const [activities, setActivities] = useState<IActivity[]>([])
  const [selectedActivity, setSelectedActivity] = useState<IActivity | null>(null)
  const [editMode, setEditMode] = useState(false)
  const [loading, setLoading] = useState(true)
  const [submitting, setSubmitting] = useState(false)
  const [target, setTarget] = useState('')

  const handleOpenCreateForm = () => { setSelectedActivity(null); setEditMode(true) }

  const handleDeleteActivity = async (event: SyntheticEvent<HTMLButtonElement>, id: string) => {
    setSubmitting(true)
    setTarget(event.currentTarget.name)
    await agent.Activities.delete(id)
    setActivities([...activities.filter(a => a.id !== id)]); setSubmitting(false)
  }

  const handleSelectActivity = (id: string) => {
    setSelectedActivity(activities.filter(a => a.id === id)[0]); setEditMode(false); window.scrollTo(0,0)
  }

  const handleCreateActivity = async (activity: IActivity) => {
    setSubmitting(true)
    await agent.Activities.create(activity)
    setActivities([...activities, activity]); setSelectedActivity(activity); setEditMode(false); setSubmitting(false)
  }

  const handleEditActivity = async (activity: IActivity) => {
    setSubmitting(true)
    await agent.Activities.update(activity)
    setActivities([...activities.filter(a => a.id !== activity.id), activity]); setSelectedActivity(activity); setEditMode(false); setSubmitting(false)
  }

  const formatDate = (activities: IActivity[]): IActivity[] => {
    let newActivities: IActivity[] = []
    activities.forEach(activity => { activity.date = activity.date.split('.')[0]; newActivities.push(activity) })
    return newActivities
  }

  useEffect(() => {
    agent.Activities.list()
      .then(response => setActivities(formatDate(response)))
      .then(() => setLoading(false));  // <= essential semicolon
  }, [])

  if (loading) return <LoaderComponent content='Loading activities...' />

  return (
    <Fragment>
      <NavBar openCreateForm={handleOpenCreateForm} />
      <Container style={{ marginTop: '7em' }}>
        <ActivityDashboard
          activities={activities} selectActivity={handleSelectActivity} selectedActivity={selectedActivity} editMode={editMode} setEditMode={setEditMode}
          setSelectedActivity={setSelectedActivity} createActivity={handleCreateActivity} editActivity={handleEditActivity}
          deleteActivity={handleDeleteActivity} submitting={submitting} target={target}
        />
      </Container>
    </Fragment>
  )
}

export default App

// notes: 
// useEffect doesn't go well with async/await, 
// will only work when async/await is wrapped in another (fetchData) function.
