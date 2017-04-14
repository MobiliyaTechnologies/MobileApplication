using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CSU_PORTABLE.Models;
using Android.Support.V7.Widget;
using CSU_PORTABLE.Droid.Utils;
using CSU_PORTABLE.Utils;
using Android.Util;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;

namespace CSU_PORTABLE.Droid.UI
{
    class StudentFragment : Fragment
    {
        const string TAG = "StudentFragment";
        private TextView textViewInfo;
        LinearLayout layoutProgress;
        LinearLayout layoutSelectClassroom;
        LinearLayout layoutSelectTemperature;
        LinearLayout layoutSubmit;
        ImageView buttonNext;
        ImageView buttonBack;
        ImageView buttonSubmit;
        Button buttonDone;

        TextView textViewQuestion;
        TextView textViewQuestionDescription;
        TextView textViewProgressMessage;
        TextView textViewSelectedClass;

        TextView textViewTooHot;
        TextView textViewHot;
        TextView textViewNormal;
        TextView textViewCold;
        TextView textViewTooCold;
        //Toast toast;
        List<ClassRoomModel> classList = null;
        List<QuestionModel> questionList = null;
        RecyclerView mRecyclerView;

        string selectedClass;
        string selectedAnswer;

        int selectedQuestionId = -1;
        int selectedClassId = -1;
        int selectedAnswerId = -1;
        int userId;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.student_dashboard, container, false);

            textViewInfo = view.FindViewById<TextView>(Resource.Id.textViewInfo);
            layoutProgress = view.FindViewById<LinearLayout>(Resource.Id.layout_progress);
            layoutSelectClassroom = view.FindViewById<LinearLayout>(Resource.Id.layout_select_classroom);
            layoutSelectTemperature = view.FindViewById<LinearLayout>(Resource.Id.layout_select_temperature);
            layoutSubmit = view.FindViewById<LinearLayout>(Resource.Id.layout_submit);

            textViewQuestion = view.FindViewById<TextView>(Resource.Id.textViewQuestion);
            textViewQuestionDescription = view.FindViewById<TextView>(Resource.Id.textViewQuestionDescription);
            textViewProgressMessage = view.FindViewById<TextView>(Resource.Id.textView_progressMessage);
            textViewSelectedClass = view.FindViewById<TextView>(Resource.Id.textView_selectedClass);

            textViewTooHot = view.FindViewById<TextView>(Resource.Id.textView_tooHot);
            textViewHot = view.FindViewById<TextView>(Resource.Id.textView_hot);
            textViewNormal = view.FindViewById<TextView>(Resource.Id.textView_feelingNormal);
            textViewCold = view.FindViewById<TextView>(Resource.Id.textView_cold);
            textViewTooCold = view.FindViewById<TextView>(Resource.Id.textView_tooCold);
            buttonNext = view.FindViewById<ImageView>(Resource.Id.buttonNext);
            buttonBack = view.FindViewById<ImageView>(Resource.Id.buttonBack);
            buttonSubmit = view.FindViewById<ImageView>(Resource.Id.buttonSubmit);
            buttonDone = view.FindViewById<Button>(Resource.Id.buttonDone);

            /*
            textViewTooHot.SetTextColor(Android.Graphics.Color.Gray);
            textViewCold.SetTextColor(Android.Graphics.Color.Gray);
            textViewNormal.SetTextColor(Android.Graphics.Color.Gray);
            textViewHot.SetTextColor(Android.Graphics.Color.Gray);
            textViewTooCold.SetTextColor(Android.Graphics.Color.Gray);
            */

            textViewTooHot.Alpha = 0.5f;
            textViewHot.Alpha = 0.5f;
            textViewNormal.Alpha = 0.5f;
            textViewCold.Alpha = 0.5f;
            textViewTooCold.Alpha = 0.5f;

            textViewTooHot.Click += delegate
            {
                selectedAnswer = "Too Hot";
                selectedAnswerId = (int)textViewTooHot.Tag;

                textViewTooHot.Alpha = 1f;
                textViewHot.Alpha = 0.5f;
                textViewNormal.Alpha = 0.5f;
                textViewCold.Alpha = 0.5f;
                textViewTooCold.Alpha = 0.5f;

                layoutSelectTemperature.SetBackgroundColor(new Android.Graphics.Color(214, 69, 66));

                /*
                textViewTooHot.SetTextColor(Android.Graphics.Color.White);
                textViewHot.SetTextColor(Android.Graphics.Color.Gray);
                textViewNormal.SetTextColor(Android.Graphics.Color.Gray);
                textViewCold.SetTextColor(Android.Graphics.Color.Gray);
                textViewTooCold.SetTextColor(Android.Graphics.Color.Gray);

                textViewTooHot.SetTypeface(textViewTooHot.Typeface, Android.Graphics.TypefaceStyle.Bold);
                textViewHot.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewNormal.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewCold.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewTooCold.SetTypeface(textViewTooCold.Typeface, Android.Graphics.TypefaceStyle.Normal);
                */
            };

            textViewHot.Click += delegate
            {
                selectedAnswer = "Hot";
                selectedAnswerId = (int)textViewHot.Tag;

                textViewTooHot.Alpha = 0.5f;
                textViewHot.Alpha = 1.0f;
                textViewNormal.Alpha = 0.5f;
                textViewCold.Alpha = 0.5f;
                textViewTooCold.Alpha = 0.5f;

                layoutSelectTemperature.SetBackgroundColor(new Android.Graphics.Color(204, 84, 48));

                /*
                textViewTooHot.SetTextColor(Android.Graphics.Color.Gray);
                textViewHot.SetTextColor(Android.Graphics.Color.White);
                textViewNormal.SetTextColor(Android.Graphics.Color.Gray);
                textViewCold.SetTextColor(Android.Graphics.Color.Gray);
                textViewTooCold.SetTextColor(Android.Graphics.Color.Gray);

                textViewTooHot.SetTypeface(textViewTooHot.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewHot.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Bold);
                textViewNormal.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewCold.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewTooCold.SetTypeface(textViewTooCold.Typeface, Android.Graphics.TypefaceStyle.Normal);
                */
            };

            textViewNormal.Click += delegate
            {
                selectedAnswer = "Feeling Normal";
                selectedAnswerId = (int)textViewNormal.Tag;

                textViewTooHot.Alpha = 0.5f;
                textViewHot.Alpha = 0.5f;
                textViewNormal.Alpha = 1f;
                textViewCold.Alpha = 0.5f;
                textViewTooCold.Alpha = 0.5f;

                layoutSelectTemperature.SetBackgroundColor(new Android.Graphics.Color(33, 77, 43));

                /*
                textViewTooHot.SetTextColor(Android.Graphics.Color.Gray);
                textViewHot.SetTextColor(Android.Graphics.Color.Gray);
                textViewNormal.SetTextColor(Android.Graphics.Color.White);
                textViewCold.SetTextColor(Android.Graphics.Color.Gray);
                textViewTooCold.SetTextColor(Android.Graphics.Color.Gray);

                textViewTooHot.SetTypeface(textViewTooHot.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewHot.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewNormal.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Bold);
                textViewCold.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewTooCold.SetTypeface(textViewTooCold.Typeface, Android.Graphics.TypefaceStyle.Normal);
                */
            };
            textViewCold.Click += delegate
            {
                selectedAnswer = "Cold";
                selectedAnswerId = (int)textViewCold.Tag;

                textViewTooHot.Alpha = 0.5f;
                textViewHot.Alpha = 0.5f;
                textViewNormal.Alpha = 0.5f;
                textViewCold.Alpha = 1f;
                textViewTooCold.Alpha = 0.5f;

                layoutSelectTemperature.SetBackgroundColor(new Android.Graphics.Color(16, 84, 86));
                /*
                textViewTooHot.SetTextColor(Android.Graphics.Color.Gray);
                textViewHot.SetTextColor(Android.Graphics.Color.Gray);
                textViewNormal.SetTextColor(Android.Graphics.Color.Gray);
                textViewCold.SetTextColor(Android.Graphics.Color.White);
                textViewTooCold.SetTextColor(Android.Graphics.Color.Gray);

                textViewTooHot.SetTypeface(textViewTooHot.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewHot.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewNormal.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewCold.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Bold);
                textViewTooCold.SetTypeface(textViewTooHot.Typeface, Android.Graphics.TypefaceStyle.Normal);
                */
            };
            textViewTooCold.Click += delegate
            {
                selectedAnswer = "Too Cold";
                selectedAnswerId = (int)textViewTooCold.Tag;

                textViewTooHot.Alpha = 0.5f;
                textViewHot.Alpha = 0.5f;
                textViewNormal.Alpha = 0.5f;
                textViewCold.Alpha = 0.5f;
                textViewTooCold.Alpha = 1f;

                layoutSelectTemperature.SetBackgroundColor(new Android.Graphics.Color(36, 116, 169));

                /*
                textViewTooHot.SetTextColor(Android.Graphics.Color.Gray);
                textViewHot.SetTextColor(Android.Graphics.Color.Gray);
                textViewNormal.SetTextColor(Android.Graphics.Color.Gray);
                textViewCold.SetTextColor(Android.Graphics.Color.Gray);
                textViewTooCold.SetTextColor(Android.Graphics.Color.White);

                textViewTooHot.SetTypeface(textViewTooHot.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewHot.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewNormal.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewCold.SetTypeface(textViewNormal.Typeface, Android.Graphics.TypefaceStyle.Normal);
                textViewTooCold.SetTypeface(textViewTooHot.Typeface, Android.Graphics.TypefaceStyle.Bold);
                */
            };

            buttonNext.Click += delegate
            {
                Log.Debug(TAG, "Next button click");
                if (selectedClass != null)
                {
                    showLayoutSelectTemperature();
                    ShowTemperatureQuestion();
                }
                else
                {
                    Utils.Utils.ShowToast(this.Context, "Please select a Classroom");
                    //ShowToast("Please select a Classroom");
                }
            };
            buttonBack.Click += delegate
            {
                Log.Debug(TAG, "Back button click");
                selectedClass = null;
                selectedClassId = -1;
                textViewSelectedClass.Text = "";
                showLayoutSelectClass();
            };
            buttonSubmit.Click += delegate
            {
                Log.Debug(TAG, "Submit button click");
                if (selectedAnswer != null)
                {
                    showLayoutSubmit();
                    submitFeedback(userId);
                }
                else
                {
                    Utils.Utils.ShowToast(this.Context, "Please select an option");
                    //ShowToast("Please select an option");
                }
            };
            buttonDone.Click += delegate
            {
                Log.Debug(TAG, "Done button click");
                selectedClass = null;
                selectedClassId = -1;
                textViewSelectedClass.Text = "";
                selectedAnswer = null;
                showLayoutSelectClass();
            };

            showLayoutProgress("Loading...");
            mRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);

            var preferenceHandler = new PreferenceHandler();
            userId = preferenceHandler.GetUserDetails().User_Id;
            if (userId != -1)
            {
                bool isNetworkEnabled = Utils.Utils.IsNetworkEnabled(this.Activity);
                if (isNetworkEnabled)
                {
                    getClassList(userId);
                    getQuestionList(userId);
                }
                else
                {
                    Utils.Utils.ShowToast(this.Context, "Please enable your internet connection !");
                    //ShowToast("Please enable your internet connection !");
                    showLayoutInfo();
                }
            }
            else
            {
                Utils.Utils.ShowToast(this.Context, "Invalid User Id. Please Login Again !");
                //ShowToast("Invalid User Id. Please Login Again !");
                showLayoutInfo();
            }

            return view;
        }

        private async void submitFeedback(int userId)
        {
            FeedbackModel feedbackModel = new FeedbackModel();
            feedbackModel.QuestionId = selectedQuestionId;
            feedbackModel.ClassId = selectedClassId;
            feedbackModel.AnswerId = selectedAnswerId;
            feedbackModel.FeedbackDesc = selectedAnswer;

            Log.Debug(TAG, "Login() " + feedbackModel.ToString());
            showLayoutSubmittingFeedback("Submitting feedback...");
            var response = await InvokeApi.Invoke(Constants.API_GIVE_FEEDBACK, JsonConvert.SerializeObject(feedbackModel), HttpMethod.Post);
            if (response.StatusCode != 0)
            {
                Log.Debug(TAG, "async Response : " + response.ToString());
                this.Activity.RunOnUiThread(() =>
                {
                    submitFeedbackResponse(response);
                });
            }
        }

        private async void submitFeedbackResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                string strContent = await restResponse.Content.ReadAsStringAsync();
                GeneralResponseModel response = JsonConvert.DeserializeObject<GeneralResponseModel>(strContent);
                if (response.Status_Code == Constants.STATUS_CODE_SUCCESS)
                {
                    Log.Debug(TAG, "Feedback Successful");
                    showLayoutSubmit();
                }
                else
                {
                    Log.Debug(TAG, "Feedback Failed");
                    showLayoutSelectTemperature();
                    Utils.Utils.ShowToast(this.Context, "Failed to submit feedback, please try again!");
                }
            }
            else
            {
                Log.Debug(TAG, "Feedback Failed");
                showLayoutSelectTemperature();
                Utils.Utils.ShowToast(this.Context, "Failed to submit feedback, please try again!");
            }
        }

        private void ShowTemperatureQuestion()
        {
            if (questionList != null && questionList.Count > 0)
            {
                QuestionModel queModel = questionList[0];
                selectedQuestionId = queModel.QuestionId;
                textViewQuestion.Text = queModel.QuestionDesc;

                List<AnswerModel> ansList = queModel.Answers;
                if (ansList != null && ansList.Count > 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        switch (i + 1)
                        {
                            case 1:
                                textViewTooCold.Text = ansList[i].AnswerDesc;
                                textViewTooCold.Tag = ansList[i].AnswerId;
                                break;
                            case 2:
                                textViewCold.Text = ansList[i].AnswerDesc;
                                textViewCold.Tag = ansList[i].AnswerId;
                                break;
                            case 3:
                                textViewNormal.Text = ansList[i].AnswerDesc;
                                textViewNormal.Tag = ansList[i].AnswerId;
                                break;
                            case 4:
                                textViewHot.Text = ansList[i].AnswerDesc;
                                textViewHot.Tag = ansList[i].AnswerId;
                                break;
                            case 5:
                                textViewTooHot.Text = ansList[i].AnswerDesc;
                                textViewTooHot.Tag = ansList[i].AnswerId;
                                break;

                        }
                    }
                }
            }
        }

        private void showLayoutProgress(string message)
        {
            textViewProgressMessage.Text = message;
            textViewInfo.Visibility = ViewStates.Gone;
            layoutProgress.Visibility = ViewStates.Visible;
            layoutSelectClassroom.Visibility = ViewStates.Gone;
            layoutSelectTemperature.Visibility = ViewStates.Gone;
            layoutSubmit.Visibility = ViewStates.Gone;
        }

        private void showLayoutSubmittingFeedback(string message)
        {
            textViewProgressMessage.Text = message;
            textViewInfo.Visibility = ViewStates.Gone;
            layoutProgress.Visibility = ViewStates.Visible;
            layoutSelectClassroom.Visibility = ViewStates.Gone;
            layoutSelectTemperature.Visibility = ViewStates.Gone;
            layoutSubmit.Visibility = ViewStates.Visible;
        }
        private void showLayoutSelectClass()
        {
            textViewInfo.Visibility = ViewStates.Gone;
            layoutProgress.Visibility = ViewStates.Gone;
            layoutSelectClassroom.Visibility = ViewStates.Visible;
            layoutSelectTemperature.Visibility = ViewStates.Gone;
            layoutSubmit.Visibility = ViewStates.Gone;
        }
        private void showLayoutSelectTemperature()
        {
            textViewInfo.Visibility = ViewStates.Gone;
            layoutProgress.Visibility = ViewStates.Gone;
            layoutSelectClassroom.Visibility = ViewStates.Gone;
            layoutSelectTemperature.Visibility = ViewStates.Visible;
            layoutSubmit.Visibility = ViewStates.Gone;
        }
        private void showLayoutSubmit()
        {
            textViewInfo.Visibility = ViewStates.Gone;
            layoutProgress.Visibility = ViewStates.Gone;
            layoutSelectClassroom.Visibility = ViewStates.Gone;
            layoutSelectTemperature.Visibility = ViewStates.Gone;
            layoutSubmit.Visibility = ViewStates.Visible;
        }
        private void showLayoutInfo()
        {
            textViewInfo.Visibility = ViewStates.Visible;
            layoutProgress.Visibility = ViewStates.Gone;
            layoutSelectClassroom.Visibility = ViewStates.Gone;
            layoutSelectTemperature.Visibility = ViewStates.Gone;
            layoutSubmit.Visibility = ViewStates.Gone;
        }

        private async void getClassList(int userId)
        {
            Log.Debug(TAG, "getAlertList()");
            var response = await InvokeApi.Invoke(Constants.API_GET_CLASS_ROOMS + "/" + userId, string.Empty, HttpMethod.Get);
            if (response.StatusCode != 0)
            {
                Log.Debug(TAG, "async Response : " + response.ToString());
                this.Activity.RunOnUiThread(() =>
                {
                    getClassListResponse(response);
                });
            }
        }

        private async void getClassListResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                string strContent = await restResponse.Content.ReadAsStringAsync();
                JArray array = JArray.Parse(strContent);
                classList = array.ToObject<List<ClassRoomModel>>();

                showClasses();
            }
            else
            {
                Log.Debug(TAG, "getAlertListResponse() Failed");
                Utils.Utils.ShowToast(this.Context, "getAlertListResponse() Failed");
                showLayoutInfo();
            }
        }

        private async void getQuestionList(int userId)
        {
            Log.Debug(TAG, "getAlertList()");
            var response = await InvokeApi.Invoke(Constants.API_GET_QUESTION_ANSWERS + "/" + userId, string.Empty, HttpMethod.Get);
            if (response.StatusCode != 0)
            {
                Log.Debug(TAG, "async Response : " + response.ToString());
                this.Activity.RunOnUiThread(() =>
                {
                    getQuestionListResponse(response);
                });
            }
        }

        private async void getQuestionListResponse(HttpResponseMessage restResponse)
        {
            if (restResponse != null && restResponse.StatusCode == System.Net.HttpStatusCode.OK && restResponse.Content != null)
            {
                Log.Debug(TAG, restResponse.Content.ToString());
                string strContent = await restResponse.Content.ReadAsStringAsync();
                JArray array = JArray.Parse(strContent);
                questionList = array.ToObject<List<QuestionModel>>();

                showClasses();
            }
            else
            {
                Log.Debug(TAG, "getAlertListResponse() Failed");
                Utils.Utils.ShowToast(this.Context, "getAlertListResponse() Failed");
                showLayoutInfo();
            }
        }

        private void showClasses()
        {
            if (classList != null && questionList != null)
            {

                // Plug in the linear layout manager:
                RecyclerView.LayoutManager mLayoutManager = new LinearLayoutManager(this.Activity);
                mRecyclerView.SetLayoutManager(mLayoutManager);

                // Plug in my adapter:
                StudentDashboardAdapter mAdapter = new StudentDashboardAdapter(classList);
                mAdapter.ItemClick += OnItemClick;
                mRecyclerView.SetAdapter(mAdapter);
                showLayoutSelectClass();
            }
            else
            {
                showLayoutInfo();
            }
        }

        void OnItemClick(object sender, int position)
        {
            int num = position + 1;
            selectedClass = classList[position].ClassDescription;
            textViewSelectedClass.Text = selectedClass;
            selectedClassId = classList[position].ClassId;
            //ShowToast("You are in " + selectedClass);
        }

       
    }
}