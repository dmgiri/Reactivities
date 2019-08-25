import React, { Fragment, useState, useEffect, SyntheticEvent, useContext } from "react"
import { Container } from "semantic-ui-react"
import { IActivity } from "../models/activity"
import NavBar from "../../features/nav/NavBar"
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard'
import agent from '../api/agent'
import LoaderComponent from './LoaderComponent'
import ActivityStore from '../stores/activityStore'
import { observer } from 'mobx-react-lite'


const App: React.FC = () => {

  const activityStore = useContext(ActivityStore)
  const [activities, setActivities] = useState<IActivity[]>([])
  const [selectedActivity, setSelectedActivity] = useState<IActivity | null>(null)
  const [editMode, setEditMode] = useState(false)
  // eslint-disable-next-line
  const [loading, setLoading] = useState(true)
  const [submitting, setSubmitting] = useState(false)
  const [target, setTarget] = useState('')

  const handleSelectActivity = (id: string) => {
    setSelectedActivity(activities.filter(a => a.id === id)[0]); setEditMode(false); window.scrollTo(0,0)
  }

  const handleEditActivity = async (activity: IActivity) => {
    setSubmitting(true)
    await agent.Activities.update(activity)
    setActivities([...activities.filter(a => a.id !== activity.id), activity]); setSelectedActivity(activity); setEditMode(false); setSubmitting(false)
  }

  const handleDeleteActivity = async (event: SyntheticEvent<HTMLButtonElement>, id: string) => {
    setSubmitting(true)
    setTarget(event.currentTarget.name)
    await agent.Activities.delete(id)
    setActivities([...activities.filter(a => a.id !== id)]); setSubmitting(false)
  }

  useEffect(() => { activityStore.loadActivities() }, [activityStore])

  if (activityStore.loadingInitial) return <LoaderComponent content='Loading activities...' />

  return (
    <Fragment>
      <NavBar />
      <Container style={{ marginTop: '7em' }}>
        <ActivityDashboard
          activities={activityStore.activities} selectActivity={handleSelectActivity} 
          setEditMode={setEditMode} setSelectedActivity={setSelectedActivity} editActivity={handleEditActivity}
          deleteActivity={handleDeleteActivity} submitting={submitting} target={target}
        />
      </Container>
    </Fragment>
  )
}

export default observer(App)
